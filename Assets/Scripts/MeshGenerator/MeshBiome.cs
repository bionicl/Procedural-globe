using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBiome : MonoBehaviour
{
    public static List<BiomeInfo>[,] GenerateBiome(int chunkSize, int seed, Vector2Int offset, PerlinNoiseSettings temperaturePerlinNoise, PerlinNoiseSettings rainfallPerlinNoise, Biome[] biomes) {
        float[,] temperatureNoise = Noise.GenerateNoiseMap(chunkSize + 1, GameManager.seed, offset, temperaturePerlinNoise, true);
        //mapDisplay.DrawNoiseMap(noise);
        float[,] rainfallNoise = Noise.GenerateNoiseMap(chunkSize + 1, GameManager.seed, offset, rainfallPerlinNoise, true);

        List<BiomeInfo>[,] output = new List<BiomeInfo>[chunkSize, chunkSize];
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                List<BiomeInfo> biomeInfos = new List<BiomeInfo>();
                foreach (var biome in biomes) {
                    if (biome.temperatureRange.Min < temperatureNoise[x, y] && biome.temperatureRange.Max >= temperatureNoise[x, y]) {
                        if (biome.rainFallRange.Min < rainfallNoise[x, y] && biome.rainFallRange.Max >= rainfallNoise[x, y]) {
                            biomeInfos.Add(new BiomeInfo(biome, 1));
                            break;
                        }
                    }
                }
                if (biomeInfos.Count == 0)
                    Debug.Log("O NIEEE:  Temp" + temperatureNoise[x, y] + "; Rainfall: " + rainfallNoise[x, y]);
                output[x, y] = biomeInfos;
            }
        }
        
        
        return output;
    }
}
