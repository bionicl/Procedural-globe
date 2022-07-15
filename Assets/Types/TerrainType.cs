using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public bool useGradient;
    [ConditionalField(nameof(useGradient), true)] public Color color;
    [ConditionalField(nameof(useGradient))] public Gradient gradient;
}
