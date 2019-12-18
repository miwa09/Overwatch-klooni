using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossReaperGun : MonoBehaviour
{
    public Transform target;
    public float targetRange = 30f;
    Vector3 shootPos;
    float damage;
    public float shotPelletCount = 20;
    public float damageMin = 2.1f;
    public float damageMax = 7;
    public float gunRangeMax = 20;
    public float gunRangeMin = 10;
    public float spreadAngle = 20.1f;
    Vector3 direction;
    int ammo;
    int maxAmmo = 8;
    float reloadTime = 1.5f;
    float reloadTimer = 0;
    public bool canShoot = true;
    public float fireRate = 0.75f;
    float fireRateTimer = 0;
    RaycastHit hit;
    public LayerMask playerLayer;
    public GameObject trails;

    void Start()
    {
        ammo = maxAmmo;
        spreadAngle /= 2;
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
        //if (Input.GetKeyDown(KeyCode.L) && canShoot) { //Testing purposes
        //    FireWeapon();
        //}
    }

    void Reload() {
        reloadTimer += Time.deltaTime;
        if (reloadTimer >= reloadTime) {
            reloadTimer = 0;
            ammo = maxAmmo;
            return;
        }
    }

    public void FireWeapon() {
        ammo--;
        canShoot = false;
        for (int i = 0; i < shotPelletCount; i++) {
            CalculateDamage();
            RandomizeAngle();
            var trail = Instantiate(trails, transform.position, transform.rotation);
            trail.transform.forward = direction; //Tracer
            trail.GetComponent<ParticleSystem>().Emit(1);
            Destroy(trail, 1);
            Debug.DrawRay(transform.position, direction * 100, Color.red, 2);
            if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, playerLayer)) //Raycast to see what was hit and where
                {
                Collider target = hit.collider; //Naming a bunch of variables
                float distance = hit.distance;
                Vector3 location = hit.point;
                GameObject targetGameObject = hit.collider.gameObject;
                if (target.tag == "Player") //To check that it's an enemy
                {
                    CalculateDamage();
                    target.GetComponent<IDamageable>().TakeDamage(damage);
                    if (GetComponent<Enemy>().hitpoints<GetComponent<Enemy>().maxHitpoints) {
                        GetComponent<EnemyBossReaper>().Lifesteal(damage);
                    }
                }
            }
        }
    }

    //Calculating gun damage and spread
    void CalculateDamage() {
        var distance = Vector3.Distance(target.position, shootPos);
        if (distance >= gunRangeMax) {
            damage = damageMin;
        }
        if (distance <= gunRangeMin) {
            damage = damageMax;
        }
        if (distance < gunRangeMax && distance > gunRangeMin) {
            damage = damageMax * (gunRangeMin / distance);
            damage = Mathf.RoundToInt(damage);
        }
    }

    void RandomizeAngle() {
        direction = Vector3.forward;
        float deviation = Random.Range(0f, spreadAngle);
        float angle = Random.Range(0f, 360f);
        direction = Quaternion.AngleAxis(deviation, Vector3.up) * direction;
        direction = Quaternion.AngleAxis(angle, Vector3.forward) * direction;
        direction = transform.rotation * direction;
    }
}
