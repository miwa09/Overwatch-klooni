using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    public GameObject spawnedItem; //The item in question that needs be spawned. Add it through the Unity editor.
    float itemTimer = 0f;
    float ticker = 1f;
    float itemSpawnTicker = 0f;
    public float itemSpawnTime = 30f;
    Vector3 itemOffset = new Vector3(0, 0.5f, 0); //Makes sure the item pickups are above the spawner
    Quaternion itemRotation = new Quaternion(0, 0, 0, 0);
    public bool itemTaken;

    void Start()
    {
        SpawnItem(); //Spawn the item as soon as the spawner becomes active
    }

    void Update()
    {
        //print("itemTimer: " + itemTimer + "itemSpawnTicker: " + itemSpawnTicker); //For testing purposes
        itemTimer += Time.deltaTime;
        while (itemTimer >= ticker)
        {
            itemTimer -= ticker;
            if (itemTaken)
            {
                itemSpawnTicker += ticker;
            }
        }
        if (itemSpawnTicker >= itemSpawnTime)
        {
            itemSpawnTicker = 0;
            SpawnItem();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SpawnItem();
        }
    }

    public void SpawnItem()
    {
        Instantiate(spawnedItem, gameObject.transform.position + itemOffset, itemRotation, gameObject.transform);
        itemTaken = false;
    }
}
