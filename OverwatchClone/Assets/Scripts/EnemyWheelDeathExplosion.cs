using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWheelDeathExplosion : MonoBehaviour
{
    BasicEnemyMovement moveScript;
    float explosionRadius = 3;
    public LayerMask damageMask;
    float explosionDamageMax;
    float explosionDamageMin = 10;
    Enemy baseScript;
    void Start()
    {
        moveScript = GetComponent<BasicEnemyMovement>();
        baseScript = GetComponent<Enemy>();
        explosionDamageMax = moveScript.playerDamage;
    }

    void Update()
    {
        if (baseScript.hasDied) {
            Explosion();
        }
    }

    void Explosion() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);
        int i = 0;
        if (hitColliders.Length > 0) {
            while (i < hitColliders.Length) {
                float distance = Vector3.Distance(transform.position, hitColliders[i].gameObject.transform.position) - 0.5f;
                if (distance < 0) {
                    distance = 0;
                }
                print(distance);
                float explosionDamage = explosionDamageMax / distance;
                if (explosionDamage <= explosionDamageMin) {
                    explosionDamage = explosionDamageMin;
                }
                if (explosionDamage >= explosionDamageMax) {
                    explosionDamage = explosionDamageMax;
                }
                if (hitColliders[i].gameObject.tag == "Player") {
                    hitColliders[i].gameObject.GetComponent<IDamageable>().TakeDamage(explosionDamage);
                    hitColliders[i].gameObject.GetComponent<Rigidbody>().AddExplosionForce(150f, gameObject.transform.position, 4, 1);
                }
                i++;
            }
        }
        Destroy(gameObject);
    }
}
