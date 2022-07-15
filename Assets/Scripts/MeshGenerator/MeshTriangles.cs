using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTriangles : MonoBehaviour
{
    public static int[] GenerateTriangles(int chunkSize) {
        int vert = 0;
        int tris = 0;
        int[] triangles = new int[chunkSize * chunkSize * 6];

        for (int z = 0; z < chunkSize; z++) {
            for (int x = 0; x < chunkSize; x++) {
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

        return triangles;
    } 
}
