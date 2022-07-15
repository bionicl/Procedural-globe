using System.Collections;
using System.Collections.Generic;
using MyBox;
using System.Linq;
using UnityEngine;
using System;

public class MeshGenerator : MonoBehaviour
{
    
    public Material material;
    public Transform player;

    [Header("Render")]
    public int chunkSize = 80;
    public float perlinAddition = 16f;
    public int renderDistance;
    [Range(0, 2)]
    public int smoothTimes = 0;

    [Header("Biome")]
    public Biome[] biomes;
    public Biome biome;
    public PerlinNoiseSettings temperaturePerlinNoise;
    public PerlinNoiseSettings rainfallPerlinNoise;

    [Header("Debug")]
    public MapDisplay mapDisplay;

    //temp
    Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();
    Dictionary<Vector2Int, GameObject> enabledChunks = new Dictionary<Vector2Int, GameObject>();
    List<Vector2Int> keysToRemove = new List<Vector2Int>();

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

        int r = renderDistance;
        List<Vector2Int> chunksToRender = new List<Vector2Int>();

        for (int x = playerCurrentX - r; x <= playerCurrentX + r; x++) {
            for (int y = playerCurrentZ - r; y <= playerCurrentZ + r; y++) {
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
        // Check if chunk is not already generated
        if (chunks.ContainsKey(chunkPosition))
        {
            if (!enabledChunks.ContainsKey(chunkPosition))
            {
                chunks[chunkPosition].SetActive(true);
                enabledChunks.Add(chunkPosition, chunks[chunkPosition]);
            }
            return;
        }

        // Generate new game object
        GameObject newGo = new GameObject();
        newGo.transform.SetParent(transform);
        newGo.name = chunkPosition.x.ToString() + ", " + chunkPosition.y.ToString();

        // Noise
        Vector3[] vertices = new Vector3[(chunkSize + 1) * (chunkSize + 1)];
        Vector2[] uvs = new Vector2[(chunkSize + 1) * (chunkSize + 1)];

        float[,] noise = Noise.GenerateNoiseMap(chunkSize + 1, GameManager.seed, chunkPosition, biome.perlinNoiseSettings);
        //mapDisplay.DrawNoiseMap(noise);

        for (int z = 0, i = 0; z < chunkSize + 1; z++) {
            for (int x = 0; x < chunkSize + 1; x++) {
                float y = noise[x, z] * perlinAddition;
                vertices[i] = new Vector3(x + chunkPosition.x*chunkSize, y, z + chunkPosition.y * chunkSize);
                uvs[i] = new Vector2(x / (float)chunkSize, z / (float)chunkSize);
                i++;
            }
        }

        // Biome
        List<BiomeInfo>[,] biomeInfoMap = MeshBiome.GenerateBiome(chunkSize, GameManager.seed, chunkPosition, temperaturePerlinNoise, rainfallPerlinNoise, biomes);

        // Colors
        //Color[] colorMap = MeshColors.GenerateColors(chunkSize, noise, perlinAddition, biome);
        Color[] colorMap = MeshColors.GenerateColorsBasedOnBiomeInfo(chunkSize, noise, perlinAddition, biomeInfoMap);

        // Texture
        Texture2D texture = TextureGenerator.TextureFromColourMapWithSmooth(colorMap, chunkSize, smoothTimes);

        // Setup mesh
        Mesh newMesh = new Mesh();
        newMesh.Clear();
        newMesh.vertices = vertices;
        newMesh.triangles = MeshTriangles.GenerateTriangles(chunkSize); ;
        newMesh.uv = uvs;
        newMesh.RecalculateNormals();

        // Setup mesh renderer and other components
        MeshRenderer newMeshRenderer = newGo.AddComponent<MeshRenderer>();
        newMeshRenderer.sharedMaterial = new Material(material);
        newMeshRenderer.sharedMaterial.mainTexture = texture;

        MeshFilter newMeshFilter = newGo.AddComponent<MeshFilter>();
        newMeshFilter.sharedMesh = newMesh;

        MeshCollider meshCollider = newGo.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = newMesh;
        
        // Housekeeping
        chunks.Add(chunkPosition, newGo);
        enabledChunks.Add(chunkPosition, newGo);
    }
}


