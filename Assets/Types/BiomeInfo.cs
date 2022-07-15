using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeInfo
{
    public Biome biome;
    public float multiplier;

    public BiomeInfo(Biome biome, float multiplier) {
        this.biome = biome;
        this.multiplier = multiplier;
    }
}
