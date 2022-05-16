using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();
    Dictionary<Vector2Int, GameObject> enabledChunks = new Dictionary<Vector2Int, GameObject>();
    public Material material;
    public Transform player;

    int seed;

    public int chunkSize = 50;
    public float perlinMultiplier = 0.3f;
    public float perlinAddition = 2f;


    // Start is called before the first frame update
    void Start()
    {
        seed = 0;
    }

    private void Update()
    {
        int playerCurrentX = Mathf.RoundToInt(player.position.x / chunkSize);
        int playerCurrentZ = Mathf.RoundToInt(player.position.z / chunkSize);
        Vector2Int playerPosition = new Vector2Int(playerCurrentX, playerCurrentZ);
        GenerateChunk(playerPosition);
        List<Vector2Int> keysToRemove = new List<Vector2Int>();
        foreach (var item in enabledChunks)
        {
            if (item.Key != playerPosition)
            {
                keysToRemove.Add(item.Key);
            }
        }
        foreach (var item in keysToRemove)
        {
            enabledChunks[item].SetActive(false);
            enabledChunks.Remove(item);
        }
        keysToRemove.Clear();
        Debug.Log(chunks.Count + ", " + enabledChunks.Count);
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
        newGo.AddComponent<MeshRenderer>().material = material;
        MeshFilter newMeshFilter = newGo.AddComponent<MeshFilter>();

        Vector3[] vertices;
        int[] triangles;
        Mesh newMesh = new Mesh();

        vertices = new Vector3[(chunkSize + 1) * (chunkSize + 1)];

        for (int z = 0, i = 0; z < chunkSize + 1; z++)
        {
            for (int x = 0; x < chunkSize + 1; x++)
            {
                float y = Mathf.PerlinNoise(x * perlinMultiplier + seed + chunkSize * chunkPosition.x * perlinMultiplier, z * perlinMultiplier + seed + chunkSize * chunkPosition.y * perlinMultiplier) * perlinAddition;
                vertices[i] = new Vector3(x + chunkPosition.x*chunkSize, y , z + chunkPosition.y * chunkSize);
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

        newMesh.Clear();
        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        newMesh.RecalculateNormals();
        newMeshFilter.mesh = newMesh;
        chunks.Add(chunkPosition, newGo);
        enabledChunks.Add(chunkPosition, newGo);
    }

    //private void OnDrawGizmos()
    //{
    //    if (vertices == null)
    //        return;
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        Gizmos.DrawSphere(vertices[i], .1f);
    //    }
    //}
}
