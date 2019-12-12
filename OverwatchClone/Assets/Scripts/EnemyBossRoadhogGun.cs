using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossRoadhogGun : MonoBehaviour
{
    public float projectileSpeed = 80;
    public int projectileAmount = 25;
    public GameObject projectilePrefab;
    public float fireRate = 1.5f;
    float fireRateTimer = 0;
    public float reloadTime = 2;
    float reloadTimer = 0;
    public int maxAmmo = 6;
    bool canShoot = true;
    int ammo;
    public Transform target;
    void Start()
    {
        ammo = maxAmmo;
    }

    
    void Update()
    {
        if (ammo <= 0) {
            Reload();
        }
        if (!canShoot) {
            fireRateTimer += Time.deltaTime;
            if (fireRateTimer >= fireRate) {
                fireRateTimer -= fireRate;
                canShoot = true;
            }
        }
        
    }

    public void FireWeapon() {
        if (ammo > 0 && canShoot) {
            ammo--;
            var projectileGroup = Instantiate(projectilePrefab, transform.position, transform.rotation);
            Destroy(projectileGroup, 5);
            canShoot = false;
        }
    }

    void Reload() {
        reloadTimer += Time.deltaTime;
        if (reloadTimer >= reloadTime) {
            reloadTimer = 0;
            ammo = maxAmmo;
            return;
        }
    }
    
}
