using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Game-related variables
    public static int seed;


    private void Awake() {
        instance = this;

        GenerateSeed();
    }

    void GenerateSeed() {
        seed = Random.Range(0, 1000000);
    }
}
