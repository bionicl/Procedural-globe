using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshColors : MonoBehaviour {
    public static Color[] GenerateColors(int chunkSize, float[,] noise, float perlinAddition, Biome biome) {
        Color[] colorMap = new Color[chunkSize * chunkSize];

        for (int y = 0; y < chunkSize; y++) {
            for (int x = 0; x < chunkSize; x++) {
                float currentHeight = noise[x, y] * perlinAddition;
                colorMap[y * chunkSize + x] = GetColorForTerrainAndHeight(biome, currentHeight);
            }
        }

        return colorMap;
    }

    public static Color[] GenerateColorsBasedOnBiomeInfo(int chunkSize, float[,] noise, float perlinAddition, List<BiomeInfo>[,] biomeInfoMap) {
        Color[] colorMap = new Color[chunkSize * chunkSize];

        for (int y = 0; y < chunkSize; y++) {
            for (int x = 0; x < chunkSize; x++) {
                float height = noise[x, y] * perlinAddition;
                Color color = GetColorForPixel(height, biomeInfoMap[x, y]);
                colorMap[y * chunkSize + x] = color;
            }
        }

        return colorMap;

    }

    static Color GetColorForPixel(float height, List<BiomeInfo> biomeInfos) {
        Color output = new Color(0, 0, 0, 1);

        foreach (var biomeInfo in biomeInfos) {
            Color newColor = GetColorForTerrainAndHeight(biomeInfo.biome, height);
            output += newColor * biomeInfo.multiplier;
        }
        output.a = 1;
        return output;
    }

    static Color GetColorForTerrainAndHeight(Biome biome, float height) {
        for (int i = 0; i < biome.regions.Length; i++) {
            if (height <= biome.regions[i].height) {
                if (biome.regions[i].useGradient && i >= 1) {
                    float start = biome.regions[i - 1].height;
                    float end = biome.regions[i].height;
                    return biome.regions[i].gradient.Evaluate((height - start) / end);
                } else {
                    return biome.regions[i].color;
                }
            }
        }
        return Color.magenta;
    }
}
