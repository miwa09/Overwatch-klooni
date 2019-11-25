using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float hitpoints = 100f;
    void Start()
    {
        
    }

    void Update()
    {
        if (hitpoints <= 0)
        {
            Destroy(gameObject);
        }
        print(hitpoints);
    }
}
