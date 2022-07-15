using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshColors : MonoBehaviour
{
    public static Color[] GenerateColors(int chunkSize, float[,] noise, float perlinAddition, Biome biome) {
        Color[] colorMap = new Color[chunkSize * chunkSize];

        for (int y = 0; y < chunkSize; y++) {
            for (int x = 0; x < chunkSize; x++) {
                float currentHeight = noise[x, y] * perlinAddition;
                for (int i = 0; i < biome.regions.Length; i++) {
                    if (currentHeight <= biome.regions[i].height) {

                        if (biome.regions[i].useGradient && i >= 1) {
                            float start = biome.regions[i - 1].height;
                            float end = biome.regions[i].height;
                            colorMap[y * chunkSize + x] = biome.regions[i].gradient.Evaluate((currentHeight - start) / end);
                        } else {
                            colorMap[y * chunkSize + x] = biome.regions[i].color;
                        }
                        break;
                    }
                }
            }
        }

        return colorMap;
    }
}
