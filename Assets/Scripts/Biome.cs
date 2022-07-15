using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Biome : ScriptableObject
{
    [Header("Biome selection logic")]
    public Vector2 rainFallRange;
    public Vector2 temperatureRange;

    [Header("Colors")]
    public TerrainType[] regions;

    [Header("Noise generator")]
    public float scale = 50f;
    public int octaves = 2;
    public float persistance = 0.5f;
    public float lacunarity = 4f;
}
