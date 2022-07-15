using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapSize, int seed, Vector2Int offset, Biome biome)
    {
        float[,] noiseMap = new float[mapSize, mapSize];

        System.Random random = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[biome.octaves];
        for (int i = 0; i < biome.octaves; i++) {
            float offsetX = random.Next(-100000, 10000);
            float offsetY = random.Next(-100000, 10000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (biome.scale <= 0) {
            biome.scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < mapSize; y++) {
            for (int x = 0; x < mapSize; x++) {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < biome.octaves; i++) {
                    float sampleX = (x + (mapSize - 1) * offset.x) / biome.scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y + (mapSize - 1) * offset.y) / biome.scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= biome.persistance;
                    frequency *= biome.lacunarity;
                }
                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                } else if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }
        //for (int y = 0; y < mapSize; y++) {
        //    for (int x = 0; x < mapSize; x++) {
        //        noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
        //    }
        //}

        return noiseMap;
    }
}
