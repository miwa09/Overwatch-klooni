using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            other.GetComponent<IDamageable>().TakeDamage(999);
        }
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            other.GetComponentInParent<Enemy>().EnemyKill();
        }
    }
}
