using System.Collections;
using System.Collections.Generic;
using MyBox;
using System.Linq;
using UnityEngine;
using System;

public class MeshGenerator : MonoBehaviour
{
    Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();
    Dictionary<Vector2Int, GameObject> enabledChunks = new Dictionary<Vector2Int, GameObject>();
    public Material material;
    public Transform player;

    int seed;

    public int chunkSize = 50;
    public float perlinAddition = 2f;
    public int renderDistance;
    [Range(0, 2)]
    public int smoothTimes = 0;

    [Header("Noise generator")]
    public float scale = .15f;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public MapDisplay mapDisplay;
    public ColorPalette colorPalette;

    //temp
    List<Vector2Int> keysToRemove = new List<Vector2Int>();


    // Start is called before the first frame update
    void Start()
    {
        seed = UnityEngine.Random.Range(0,1000000);
        Update();
    }

    [ButtonMethod]
    private void Reset()
    {
        foreach (var item in chunks)
        {
            Destroy(item.Value);
        }
        chunks.Clear();
        enabledChunks.Clear();
    }

    private void Update()
    {
        int playerCurrentX = Mathf.RoundToInt(player.position.x / chunkSize);
        int playerCurrentZ = Mathf.RoundToInt(player.position.z / chunkSize);
        Vector2Int playerPosition = new Vector2Int(playerCurrentX, playerCurrentZ);
        //Debug.Log(playerPosition);

        int r = renderDistance;
        List<Vector2Int> chunksToRender = new List<Vector2Int>();
        //chunksToRender.Add(playerPosition);
        for (int x = playerCurrentX - r; x <= playerCurrentX + r; x++) {
            for (int y = playerCurrentZ - r; y <= playerCurrentZ + r; y++) {
                //Debug.Log(new Vector2Int(x, y));
                if (Mathf.Pow(x - playerCurrentX, 2) + Mathf.Pow(y - playerCurrentZ, 2) <= Mathf.Pow(r, 2)) {
                    chunksToRender.Add(new Vector2Int(x, y));
                }
            }
        }

        keysToRemove.Clear();
        foreach (var item in enabledChunks)
        {
            if (!chunksToRender.Contains(item.Key))
            {
                keysToRemove.Add(item.Key);
            }
        }
        foreach (var item in keysToRemove)
        {
            enabledChunks[item].SetActive(false);
            enabledChunks.Remove(item);
        }

        foreach (var item in chunksToRender)
        {
            GenerateChunk(item);
        }
    }

    void GenerateChunk(Vector2Int chunkPosition)
    {
        if (chunks.ContainsKey(chunkPosition))
        {
            if (!enabledChunks.ContainsKey(chunkPosition))
            {
                chunks[chunkPosition].SetActive(true);
                enabledChunks.Add(chunkPosition, chunks[chunkPosition]);
            }
            return;
        }
        GameObject newGo = new GameObject();
        newGo.transform.SetParent(transform);
        newGo.name = chunkPosition.x.ToString() + ", " + chunkPosition.y.ToString();
        MeshRenderer newMeshRenderer = newGo.AddComponent<MeshRenderer>();
        newMeshRenderer.sharedMaterial = new Material(material);
        MeshFilter newMeshFilter = newGo.AddComponent<MeshFilter>();
        MeshCollider meshCollider = newGo.AddComponent<MeshCollider>();    

        Vector3[] vertices;
        int[] triangles;
        Vector2[] uvs;
        Mesh newMesh = new Mesh();
        Color[] colorMap = new Color[chunkSize * chunkSize];

        vertices = new Vector3[(chunkSize + 1) * (chunkSize + 1)];
        uvs = new Vector2[(chunkSize + 1) * (chunkSize + 1)];

        float[,] noise = Noise.GenerateNoiseMap(chunkSize + 1, seed, chunkPosition, scale, octaves, persistance, lacunarity);
        //mapDisplay.DrawNoiseMap(noise);

        for (int z = 0, i = 0; z < chunkSize + 1; z++)
        {
            for (int x = 0; x < chunkSize + 1; x++)
            {
                //float y = Mathf.PerlinNoise(x * perlinMultiplier + seed + chunkSize * chunkPosition.x * perlinMultiplier, z * perlinMultiplier + seed + chunkSize * chunkPosition.y * perlinMultiplier) * perlinAddition;
                //y += Mathf.PerlinNoise(x * perlinMultiplier + seed*200 + chunkSize*8 * chunkPosition.x * perlinMultiplier, z * perlinMultiplier + seed*200 + chunkSize*8 * chunkPosition.y * perlinMultiplier) * perlinAddition/4;
                float y = noise[x, z] * perlinAddition;
                vertices[i] = new Vector3(x + chunkPosition.x*chunkSize, y, z + chunkPosition.y * chunkSize);
                uvs[i] = new Vector2(x / (float)chunkSize, z / (float)chunkSize);
                i++;
            }
        }

        int vert = 0;
        int tris = 0;
        triangles = new int[chunkSize * chunkSize * 6];
        for (int z = 0; z < chunkSize; z++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + chunkSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + chunkSize + 1;
                triangles[tris + 5] = vert + chunkSize + 2;
                
                vert++;
                tris += 6;
            }
            vert++;
        }

        // Colors
        for (int y = 0; y < chunkSize; y++) {
            for (int x = 0; x < chunkSize; x++) {
                float currentHeight = noise[x, y] * perlinAddition;
                for (int i = 0; i < colorPalette.regions.Length; i++) {
                    if (currentHeight <= colorPalette.regions[i].height) {
                        
                        if (colorPalette.regions[i].useGradient && i >= 1) {
                            float start = colorPalette.regions[i - 1].height;
                            float end = colorPalette.regions[i].height;
                            colorMap[y * chunkSize + x] = colorPalette.regions[i].gradient.Evaluate((currentHeight - start)/end);
                        } else {
                            colorMap[y * chunkSize + x] = colorPalette.regions[i].color;
                        }
                        break;
                    }
                }
            }
        }

        newMesh.Clear();
        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.uv = uvs;
        newMesh.RecalculateNormals();
        newMeshFilter.sharedMesh = newMesh;
        newMeshRenderer.sharedMaterial.mainTexture = TextureGenerator.TextureFromColourMapWithSmooth(colorMap, chunkSize, smoothTimes);
        meshCollider.sharedMesh = newMesh;
        chunks.Add(chunkPosition, newGo);
        enabledChunks.Add(chunkPosition, newGo);
    }

    //private void OnDrawGizmos() {
    //    foreach (var item in enabledChunks) {
    //        Vector3[] vertices = item.Value.GetComponent<MeshFilter>().mesh.vertices;
    //        if (vertices == null)
    //            return;
    //        for (int i = 0; i < vertices.Length; i++) {
    //            TextGizmo.Draw(vertices[i], vertices[i].x.ToString() + ", " + vertices[i].y.ToString());
    //        }
    //    }
        
    //}
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public bool useGradient;
    [ConditionalField(nameof(useGradient), true)] public Color color;
    [ConditionalField(nameof(useGradient))] public Gradient gradient;
}
