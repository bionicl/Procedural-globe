using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[CreateAssetMenu]
public class Biome : ScriptableObject
{
    [Header("Biome selection logic")]
    //[MinMaxRange(0, 1)]
    public RangedFloat rainFallRange;
    //[MinMaxRange(0, 1)]
    public RangedFloat temperatureRange;

    [Header("Colors")]
    public TerrainType[] regions;

    [Header("Noise generator")]
    [DisplayInspector]
    public PerlinNoiseSettings perlinNoiseSettings;
}
