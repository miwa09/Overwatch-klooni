using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerBrigitteWhipshot : MonoBehaviour
{
    public GameObject master;
    public PlayerAbilitiesBrigitte abilityScript;
    public LayerMask affectedLayers;
    RaycastHit hit;
    public Transform cameraPos;
    float damage;
    public bool damageOnce = true;
    public float maxKnocback = 12;
    public float minKnockback = 5;

    private void Start() {
        damage = abilityScript.whipshotDamage;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == affectedLayers && !abilityScript.isReturning && damageOnce) {
            other.GetComponentInParent<IDamageable>().TakeDamage(damage);
            GetComponentInParent<IUltCharge>().AddUltCharge(damage);
            other.GetComponentInParent<IStunable>().DamageKnockback(other.transform.position - transform.parent.transform.position, CalculateKnockback(other.transform.position, transform.parent.transform.position), 3.4f);
            abilityScript.isReturning = true;
        }
    }
    private void FixedUpdate() {
        Physics.Raycast(cameraPos.position, cameraPos.forward, out hit, abilityScript.whipshotRange, affectedLayers);
        Vector3 location = hit.point;
        if (Vector3.Distance(transform.position, location) < cameraPos.forward.magnitude * abilityScript.whipshotSpeed * Time.fixedDeltaTime) {
            abilityScript.isReturning = true;
            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") && damageOnce) {
                damageOnce = false;
                hit.collider.GetComponentInParent<IDamageable>().TakeDamage(damage);
                GetComponentInParent<IUltCharge>().AddUltCharge(damage);
                hit.collider.GetComponentInParent<IStunable>().DamageKnockback(hit.collider.transform.position - transform.parent.transform.position, CalculateKnockback(hit.collider.transform.position, transform.parent.transform.position), 3.4f);
            }
        }
    }

    float CalculateKnockback(Vector3 hitLocation, Vector3 startLocation) {
        float knockback;
        var distance = 1 - Vector3.Distance(hitLocation, startLocation) / abilityScript.whipshotRange;
        knockback = maxKnocback * distance;
        if (knockback < minKnockback) {
            knockback = minKnockback;
        }
        if (knockback > maxKnocback) {
            knockback = maxKnocback;
        }
        return knockback;
    }
}
