using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapSize, Vector2Int offset, float scale)
    {
        float[,] noiseMap = new float[mapSize, mapSize];

        if (scale <= 0) {
            scale = 0.0001f;
        }

        for (int y = 0; y < mapSize; y++) {
            for (int x = 0; x < mapSize; x++) {
                float sampleX = (x + (mapSize - 1) * offset.x)/ scale;
                float sampleY = (y + (mapSize - 1) * offset.y) / scale;

                float perlinValue = Mathf.PerlinNoise(sampleX , sampleY ) * 2;
                //float y = Mathf.PerlinNoise(x * perlinMultiplier + seed + chunkSize * chunkPosition.x * perlinMultiplier, z * perlinMultiplier + seed + chunkSize * chunkPosition.y * perlinMultiplier) * perlinAddition;
                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }
}
