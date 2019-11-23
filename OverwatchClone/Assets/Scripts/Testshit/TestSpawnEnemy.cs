using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawnEnemy : MonoBehaviour
{
    //A tester script letting us spawn enemies in preset locations. Press F1 to spawn them in order. The script is set so you can only have as many present as there are spawns. It's not smart enough to know the order in which the enemies have been killed, so spawning 1, 2, 3, 4, 5, and 6 in a row, then killing number 6 and spawning a new one will spawn the new enemy on the same spawn as where the first enemy was spawned. If it's still alive, clipping will occur. So kill all of them or kill them in the same order as they spawn.

    public GameObject enemy;
    public Transform[] spawns;
    public Transform parent;
    Quaternion rotation = new Quaternion(0, 0, 0, 0);
    Vector3 offset = new Vector3(0, 1);
    int spawnNo = 0;
    int enemiesActive;
    bool canSpawn = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && canSpawn)
        {
            Instantiate(enemy, spawns[spawnNo].position + offset, rotation, parent);
            spawnNo++;
            if (spawnNo > spawns.Length - 1)
            {
                spawnNo = 0;
            }
        }
        enemiesActive = parent.childCount;
        if (enemiesActive >= spawns.Length)
        {
            canSpawn = false;
        }
        else canSpawn = true;
    }
}
