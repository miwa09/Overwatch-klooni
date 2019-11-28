using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilitySoldier76Rocket : MonoBehaviour
{
    public float hitDamage;
    public float explosionDamageMax = 80;
    public float explosionRadius = 3;
    public float speed = 25;
    public LayerMask damageMask;
    Vector3 hit;
    bool damageOnce = true;

    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        hit = transform.position;
        Explode(other.gameObject);
        Destroy(gameObject);
    }

    void Explode(GameObject obj)
    {
        if (obj.layer == LayerMask.NameToLayer("Enemy") && damageOnce)
        {
            obj.GetComponentInParent<Enemy>().hitpoints -= hitDamage;
            damageOnce = false;
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);
        int i = 0;

        if (hitColliders.Length > 0)
        {
            while (i < hitColliders.Length)
            {
                print("Objects hit: " + hitColliders.Length + " Name: " + hitColliders[i].gameObject.name);
                float distance = Vector3.Distance(hit, hitColliders[i].gameObject.transform.position) - 2f;
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
                }
                if (hitColliders[i].gameObject.layer == LayerMask.NameToLayer("Player1"))
                {
                        hitColliders[i].gameObject.GetComponent<IDamageable>();
                }
                i++;
                print("Distance: " + distance + " Explosion damage: " + explosionDamage);
            }
        }

    }
}
