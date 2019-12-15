using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrigitteMelee : MonoBehaviour
{
    public string inputPrefix = "P2";
    public float damage = 35;
    List<GameObject> hitOnce = new List<GameObject>();
    public Collider meleeCollider;
    bool canSwing = true;
    public Animation meleeAnimation;
    public PlayerAbilitiesBrigitte abilityScript;
    public bool disabled = false;


    void Update()
    {
        if ((Input.GetButton(inputPrefix + "PrimaryFire") || Input.GetButtonDown(inputPrefix + "Melee")) && !disabled){
            StartMeleeSwing();
        }
    }

    void StartMeleeSwing() {
        meleeCollider.enabled = true;
        meleeAnimation.Play();
        canSwing = false;
    }

    public void EndMeleeSwing() {
        hitOnce.Clear();
        meleeCollider.enabled = false;
        canSwing = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            if (!hitOnce.Contains(other.transform.parent.gameObject)) {
                hitOnce.Add(other.transform.parent.gameObject);
                other.transform.parent.GetComponent<IDamageable>().TakeDamage(damage);
                other.transform.parent.GetComponent<IStunable>().DamageKnockback(other.transform.position - Vector3.right, 3, 0.7f);
                if (other.transform.parent.GetComponent<BasicEnemyMovement>() != null) {
                    other.transform.parent.GetComponent<IStoppable>().StopMovement();

                }
            }
            abilityScript.AbilityPassive();
        }
    }
}
