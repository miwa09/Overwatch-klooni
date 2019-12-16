using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Timeline;



public class SpawnMaster : MonoBehaviour
{
    public GameObject spawnBasicEnemy;
    public SpawnStructs spawns;
    float spawnTimer = 0;
    public float spawnTicker = 2.5f;
    bool canSpawn = true;

    private void Update() {
        var timer = Time.deltaTime;

    }

    public void SpawnWave(SpawnWaveData wave) {
        for (int i = 0; i < wave.spawns.Length; i++) {
            var spawnedEnemy = Instantiate(wave.spawns[i].prefab, wave.spawns[i].spawnpoint.position, wave.spawns[i].spawnpoint.rotation);
            spawnedEnemy.GetComponent<Iai>().AddWaypoints(wave.spawns[i].waypoints);
            EnableNavMeshAgent(spawnedEnemy);
        }
    }

    private void Start() {
        SpawnWave(spawns.wave);
    }
    void EnableNavMeshAgent(GameObject obj) {
        obj.GetComponent<NavMeshAgent>().enabled = true;
        obj.GetComponent<BasicEnemyMovement>().enabled = true;
    }
    int RandomizeRoute(int minValue, int maxValue) {
        int randomValue = Random.Range(minValue, maxValue + 1);
        return randomValue;
    }
}
