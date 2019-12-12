using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectileGroup : MonoBehaviour
{
    public GameObject prefab;
    public GameObject lastSpawned;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) {
            lastSpawned = Instantiate(prefab, transform.position, transform.rotation);
            Destroy(lastSpawned, 4);
        }
        if (Input.GetKeyDown(KeyCode.J) && lastSpawned != null) {
            Destroy(lastSpawned);
        }
    }
}
