using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    static Dictionary<int, Vector2[]> octaveOffsetsDictionary = new Dictionary<int, Vector2[]>();

    static Vector2[] GetOctaveOffsets(int seed) {
        Vector2[] output;
        bool success = octaveOffsetsDictionary.TryGetValue(seed, out output);

        if (!success) {
            System.Random random = new System.Random(seed);
            output = new Vector2[6];
            for (int i = 0; i < 6; i++) {
                float offsetX = random.Next(-100000, 10000);
                float offsetY = random.Next(-100000, 10000);
                output[i] = new Vector2(offsetX, offsetY);
            }
            octaveOffsetsDictionary.Add(seed, output);
        }

        return output;
    }

    public static float[,] GenerateNoiseMap(int mapSize, int seed, Vector2Int offset, PerlinNoiseSettings noiseSettings, bool zeroToOne = false)
    {
        float[,] noiseMap = new float[mapSize, mapSize];

        Vector2[] octaveOffsets = GetOctaveOffsets(seed);

        for (int y = 0; y < mapSize; y++) {
            for (int x = 0; x < mapSize; x++) {
                noiseMap[x, y] = GetHeightBasedOnNoiseSettingsAndPosition(noiseSettings, y, x, mapSize, offset, octaveOffsets, zeroToOne);
                
            }
        }

        return noiseMap;
    }

    public static float[,] GenerateNoiseMapBasedOnBiomeInfo(int mapSize, int seed, Vector2Int offset, List<BiomeInfo>[,] biomeInfoMap, bool zeroToOne = false) {
        float[,] noiseMap = new float[mapSize, mapSize];

        Vector2[] octaveOffsets = GetOctaveOffsets(seed);

        for (int y = 0; y < mapSize; y++) {
            for (int x = 0; x < mapSize; x++) {
                float height = GetHeightForPixel(mapSize, seed, offset, biomeInfoMap[x, y], y, x, octaveOffsets);
                noiseMap[x, y] = height;

            }
        }

        return noiseMap;
    }

    public static float GetHeightForPixel(int mapSize, int seed, Vector2Int offset, List<BiomeInfo> biomeInfos, int y, int x, Vector2[] octaveOffsets) {
        float output = 0;

        foreach (var biomeInfo in biomeInfos) {
            float value = GetHeightBasedOnNoiseSettingsAndPosition(biomeInfo.biome.perlinNoiseSettings, y, x, mapSize, offset, octaveOffsets);
            output += value * biomeInfo.multiplier;
        }
        return output;
    }

    static float GetHeightBasedOnNoiseSettingsAndPosition(PerlinNoiseSettings noiseSettings, int y, int x, int mapSize, Vector2Int offset, Vector2[] octaveOffsets, bool zeroToOne = false) {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < noiseSettings.octaves; i++) {
            float sampleX = (x + (mapSize - 1) * offset.x) / noiseSettings.scale * frequency + octaveOffsets[i].x;
            float sampleY = (y + (mapSize - 1) * offset.y) / noiseSettings.scale * frequency + octaveOffsets[i].y;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            if (!zeroToOne) {
                perlinValue = (perlinValue * 2) - 1;
            }
            noiseHeight += perlinValue * amplitude;

            amplitude *= noiseSettings.persistance;
            frequency *= noiseSettings.lacunarity;
        }

        
        return noiseHeight;
    }
}
