using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PerlinNoiseSettings : ScriptableObject
{
    public float scale = 50f;
    public int octaves = 2;
    public float persistance = 0.5f;
    public float lacunarity = 4f;
}
