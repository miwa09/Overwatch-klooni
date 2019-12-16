using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilitySoldier76Rocket : MonoBehaviour
{
    public GameObject master;
    public float hitDamage;
    public float explosionDamageMax = 80;
    public float explosionRadius = 3;
    public float speed = 25;
    public LayerMask damageMask;
    Vector3 hit;
    bool damageOnce = true;

    private void FixedUpdate()
    {
        transform.position += transform.forward * Time.fixedDeltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        hit = transform.position;
        if (damageOnce) {
            Explode(other.gameObject);
        }
        Destroy(gameObject);
    }

    void Explode(GameObject obj)
    {
        if (obj.layer == LayerMask.NameToLayer("Enemy") && damageOnce)
        {
            obj.GetComponentInParent<IDamageable>().TakeDamage(hitDamage);
            master.GetComponent<IUltCharge>().AddUltCharge(hitDamage);
            damageOnce = false;
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);
        int i = 0;

        if (hitColliders.Length > 0)
        {
            while (i < hitColliders.Length)
            {
                float distance = Vector3.Distance(hit, hitColliders[i].gameObject.transform.position) - 1.5f;
                if (distance <= 0)
                {
                    distance = 0;
                }
                float explosionDamage = explosionDamageMax / distance;
                if (explosionDamage <= 40)
                {
                    explosionDamage = 40;
                }
                if (explosionDamage >= explosionDamageMax)
                {
                    explosionDamage = explosionDamageMax;
                }
                if (hitColliders[i].gameObject.layer == LayerMask.NameToLayer("Enemy") && hitColliders[i].GetComponent<EnemyColliderLocator>().isBody)
                {
                    hitColliders[i].gameObject.GetComponentInParent<IDamageable>().TakeDamage(explosionDamage);
                    hitColliders[i].gameObject.GetComponentInParent<IStunable>().DamageKnockback(hitColliders[i].transform.position - transform.position, 5, 1);
                    master.GetComponent<IUltCharge>().AddUltCharge(explosionDamage);
                }
                if (hitColliders[i].gameObject.tag == "Player")
                {
                        hitColliders[i].gameObject.GetComponent<IDamageable>().TakeDamage(explosionDamage/2);
                    hitColliders[i].gameObject.GetComponent<Rigidbody>().AddExplosionForce(10f, gameObject.transform.position, 3, 0, ForceMode.VelocityChange);
                }
                i++;
            }
        }

    }
}
