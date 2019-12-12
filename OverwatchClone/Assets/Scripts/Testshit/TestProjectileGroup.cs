using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectileGroup : MonoBehaviour
{
    public GameObject prefab;
    public GameObject lastSpawned;
    float timer = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) {
            lastSpawned = Instantiate(prefab, transform.position, transform.rotation);
            lastSpawned.transform.forward = transform.forward;
            Destroy(lastSpawned, 4);
        }
        if (Input.GetKeyDown(KeyCode.J) && lastSpawned != null) {
            Destroy(lastSpawned);
        }
        timer += Time.deltaTime;
        if (timer >= 1) {
            timer -= 1;
            var lastSpawned2 = Instantiate(prefab, transform.position, transform.rotation);
            lastSpawned2.transform.forward = transform.forward;
            Destroy(lastSpawned2, 2);
        }
    }
}
