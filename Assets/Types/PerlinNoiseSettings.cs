using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PerlinNoiseSettings : ScriptableObject
{
    [Min(0.001f)]
    public float scale = 50f;
    [Range(1,6)]
    public int octaves = 2;
    public float persistance = 0.5f;
    public float lacunarity = 4f;
}
