using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnMaster : MonoBehaviour
{
    public Transform[] spawn1path1;
    public Transform spawn1;
    public GameObject[] spawnList;
    float spawnTimer = 0;
    float spawnTicker = 1;
    bool canSpawn = true;


    public void SpawnWave1(int intensity) {
        for (int i = 0; i < spawnList.Length; i++) {
            if (canSpawn) {
                GameObject spawnedEnemy = Instantiate(spawnList[i], spawn1.position, spawn1.rotation);
                spawnedEnemy.GetComponent<BasicEnemyMovement>().waypoints = spawn1path1;
                EnableNavMeshAgent(spawnedEnemy);
                
            }
            canSpawn = false;
        }
        if (!canSpawn) {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnTicker) {
                spawnTimer = 0;
                canSpawn = true;
            }
        }
    }
    void EnableNavMeshAgent(GameObject obj) {
        obj.GetComponent<NavMeshAgent>().enabled = true;
        obj.GetComponent<BasicEnemyMovement>().enabled = true;
    }
}
