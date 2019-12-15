using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpawnWaveData {
    public SingleSpawnData[] spawns;
}

[System.Serializable]
public struct SingleSpawnData {
    public GameObject prefab;
    public Transform spawnpoint;
}


public class SpawnStructs : MonoBehaviour
{
    public SpawnWaveData wave;

    void Start() {

    }

    void Update() {

    }
}
