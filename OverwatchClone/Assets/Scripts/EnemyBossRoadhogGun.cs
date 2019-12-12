using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossRoadhogGun : MonoBehaviour
{
    public float projectileSpeed = 80;
    public int projectileAmount = 25;
    public GameObject projectilePrefab;
    public float fireRate;
    public float reloadTime = 2;
    float reloadTimer = 0;
    public int maxAmmo = 6;
    int ammo;
    void Start()
    {
        ammo = maxAmmo;
    }

    
    void Update()
    {
        
    }

    void FireWeapon() {
        
    }
    
}
