using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class MeshBiome : MonoBehaviour
{
    public static List<BiomeInfo>[,] GenerateBiome(int chunkSize, int seed, Vector2Int offset, PerlinNoiseSettings temperaturePerlinNoise, PerlinNoiseSettings rainfallPerlinNoise, Biome[] biomes) {
        float[,] temperatureNoise = Noise.GenerateNoiseMap(chunkSize + 1, seed, offset, temperaturePerlinNoise, true);
        //mapDisplay.DrawNoiseMap(noise);
        float[,] rainfallNoise = Noise.GenerateNoiseMap(chunkSize + 1, seed, offset, rainfallPerlinNoise, true);

        List<BiomeInfo>[,] output = new List<BiomeInfo>[chunkSize, chunkSize];
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                List<BiomeInfo> biomeInfos = new List<BiomeInfo>();
                foreach (var biome in biomes) {
                    if (biome.temperatureRange.Min < temperatureNoise[x, y] && biome.temperatureRange.Max >= temperatureNoise[x, y]) {
                        if (biome.rainFallRange.Min < rainfallNoise[x, y] && biome.rainFallRange.Max >= rainfallNoise[x, y]) {
                            biomeInfos.Add(new BiomeInfo(biome, 0));
                        }
                    }
                }
                output[x, y] = CheckBiomeInfoMultipliers(biomeInfos, temperatureNoise[x, y], rainfallNoise[x, y]);
            }
        }
        
        
        return output;
    }

    static List<BiomeInfo> CheckBiomeInfoMultipliers(List<BiomeInfo> input, float temperatureHeight, float rainfallHeight) {

        if (input.Count == 1) {
            input[0].multiplier = 1;
            return input;
        } else if (input.Count == 0) {
            return input;
        }
        

        RangedFloat xRange = input[0].biome.temperatureRange;
        RangedFloat yRange = input[0].biome.rainFallRange;
        
        // Find intersection area
        for (int i = 1; i < input.Count; i++) {
            RangedFloat range = input[i].biome.temperatureRange;
            var intersection = GetIntersection(xRange, range);
            if (intersection.HasValue)
                xRange = intersection.Value;

            RangedFloat range2 = input[i].biome.rainFallRange;
            var intersection2 = GetIntersection(yRange, range2);
            if (intersection2.HasValue)
                yRange = intersection2.Value;
        }
        
        // Find verteces
        Biome[] intersectionVertex = new Biome[4];
        foreach (var item in input) {
            RangedFloat tempRange = item.biome.temperatureRange;
            RangedFloat rainRange = item.biome.rainFallRange;
            if (tempRange.Min == xRange.Min && rainRange.Max == yRange.Max) {
                // Top left
                //Debug.Log("Top left");
                intersectionVertex[2] = item.biome;
            }
            if (tempRange.Max == xRange.Max && rainRange.Max == yRange.Max) {
                // Top right
                //Debug.Log("Top right");
                intersectionVertex[3] = item.biome;
            }
            if (tempRange.Min == xRange.Min && rainRange.Min == yRange.Min) {
                // Bottom left
                //Debug.Log("Bottom left");
                intersectionVertex[1] = item.biome;
            }
            if (tempRange.Max == xRange.Max && rainRange.Min == yRange.Min) {
                // Bottom right
                //Debug.Log("Bottom right");
                intersectionVertex[0] = item.biome;
            }

            if (intersectionVertex[0] == null || intersectionVertex[1] == null || intersectionVertex[2] == null || intersectionVertex[3] == null) {
                if (tempRange.Max == xRange.Max) {
                    // Right edge
                    //Debug.Log("Right edge");
                    intersectionVertex[0] = item.biome;
                    intersectionVertex[3] = item.biome;
                } else if (tempRange.Min == xRange.Min) {
                    // Left edge
                    //Debug.Log("Left edge");
                    intersectionVertex[1] = item.biome;
                    intersectionVertex[2] = item.biome;
                } else if (rainRange.Max == yRange.Max) {
                    // Top edge
                    //Debug.Log("Top edge");
                    intersectionVertex[2] = item.biome;
                    intersectionVertex[3] = item.biome;
                } else {
                    // Bottom edge
                    //Debug.Log("Bottom edge");
                    intersectionVertex[0] = item.biome;
                    intersectionVertex[1] = item.biome;
                }
            }
        }

        // Calculate biomes
        float x = (temperatureHeight - xRange.Min) / (xRange.Max - xRange.Min);
        float y = (rainfallHeight - yRange.Min) / (yRange.Max - yRange.Min);

        List<BiomeInfo> output = input;

        for (int i = 0; i < intersectionVertex.Length; i++) {
            float value = 0;
            switch (i) {
                case 0:
                    value = (1 - x) * y;
                    break;

                case 1:
                    value = x * y;
                    break;

                case 2:
                    value = x * (1 - y);
                    break;

                case 3:
                    value = (1 - x) * (1 - y);
                    break;
            }
            for (int j = 0; j < input.Count; j++) {
                if (output[j].biome == intersectionVertex[i]) {
                    output[j].multiplier += value;
                }
            }
        }
        return output;
    }


    public static RangedFloat? GetIntersection(RangedFloat range1, RangedFloat range2) {
        float greatestMin = range1.Min > range2.Min ? range1.Min : range2.Min;
        float smallestMax = range1.Max < range2.Max ? range1.Max : range2.Max;

        //no intersection
        if (greatestMin > smallestMax) {
            return null;
        }

        return new RangedFloat(greatestMin, smallestMax);
    }
}