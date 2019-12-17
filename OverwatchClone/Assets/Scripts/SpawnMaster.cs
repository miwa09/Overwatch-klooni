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
            var spawnedEnemy = Instantiate(wave.spawns[0].prefab, wave.spawns[0].spawnpoint.position, wave.spawns[0].spawnpoint.rotation);
            spawnedEnemy.GetComponent<Iai>().AddWaypoints(wave.spawns[0].waypoints);
            EnableNavMeshAgent(spawnedEnemy);
    }

    private void Start() {
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
