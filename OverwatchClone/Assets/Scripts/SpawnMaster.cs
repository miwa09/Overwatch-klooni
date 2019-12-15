using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnMaster : MonoBehaviour
{
    public GameObject spawnBasicEnemy;
    float spawnTimer = 0;
    public float spawnTicker = 2.5f;
    bool canSpawn = true;
    public Transform spawn1;
    public Transform spawn2;
    public Transform spawn3;
    public Transform spawn4;
    public Transform spawn5;
    public Transform spawn6;
    public Transform[] spawn1waypoints;
    public Transform[] spawn2waypoints;
    public Transform[] spawn3waypoints;
    public Transform[] spawn3waypoints2;
    public Transform[] spawn4waypoints;
    public Transform[] spawn5waypoints1;
    public Transform[] spawn5waypoints2;
    public Transform[] spawn6waypoints1;
    public Transform[] spawn6waypoints2;


    public void Spawn1Basic() {
        if (canSpawn) {
            GameObject spawnedEnemy = Instantiate(spawnBasicEnemy, spawn1.position, spawn1.rotation);
            spawnedEnemy.GetComponent<BasicEnemyMovement>().waypoints = spawn1waypoints;
            EnableNavMeshAgent(spawnedEnemy);
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

    public void Spawn3Basic() {
        if (canSpawn) {
            GameObject spawnedEnemy = Instantiate(spawnBasicEnemy, spawn3.position, spawn3.rotation);
            spawnedEnemy.GetComponent<BasicEnemyMovement>().waypoints = spawn3waypoints;
            EnableNavMeshAgent(spawnedEnemy);
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

    public void Spawn2Basic() {
        if (canSpawn) {
            GameObject spawnedEnemy = Instantiate(spawnBasicEnemy, spawn2.position, spawn2.rotation);
            spawnedEnemy.GetComponent<BasicEnemyMovement>().waypoints = spawn2waypoints;
            EnableNavMeshAgent(spawnedEnemy);
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
    public void Spawn5Basic() {
        if (canSpawn) {
            GameObject spawnedEnemy = Instantiate(spawnBasicEnemy, spawn5.position, spawn5.rotation);
            if (RandomizeRoute(1, 2) == 1) {
                spawnedEnemy.GetComponent<BasicEnemyMovement>().waypoints = spawn5waypoints1;
            }
            if (RandomizeRoute(1, 2) == 2) {
                spawnedEnemy.GetComponent<BasicEnemyMovement>().waypoints = spawn5waypoints2;
            }
            EnableNavMeshAgent(spawnedEnemy);
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
    int RandomizeRoute(int minValue, int maxValue) {
        int randomValue = Random.Range(minValue, maxValue + 1);
        return randomValue;
    }
}
