using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCull : MonoBehaviour
{
    float timer = 0f;
    public float cullBegin = 5f;
    public float cullTimer = 0.5f;
    public float destroyTime = 6f;
    public GameObject[] cullObjects;
    public Enemy cullEnemyScript;
    Rigidbody rig;

    private void Start() {
        rig = GetComponent<Rigidbody>();
    }

    //This script does a little bit of cleaning once an enemy has been killed. It removes unnecessary parts and sinks the mesh underground before it's deleted
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= cullBegin)
        {
            rig.isKinematic = false;
            foreach (GameObject obj in cullObjects) //You can add what objects to remove in the editor
            {
                Destroy(obj);
            }
            if (cullEnemyScript != null) //Removes the enemy.cs script so unless it has already been destroyed
            {
                Destroy(cullEnemyScript);
            }
            gameObject.transform.position += Vector3.down * Time.deltaTime; //A nice little sinking 'animation'
            Destroy(gameObject, destroyTime); //Destroy evertything once it's out of sight
        }
    }
}
