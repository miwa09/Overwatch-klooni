using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainroomTrigger : MonoBehaviour
{
    public float damage = 5f; //The damage the trigger does every tick
    bool takingDamage = true; //Cooldown so you don't take damage every frame
    float timer;
    public float damageTicker = 1f;

    private void Update()
    {
        if (!takingDamage) //Simple timer
        {
            timer += Time.deltaTime;
            if (timer >= damageTicker)
            {
                timer -= damageTicker;
                takingDamage = true;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player 1") && takingDamage || other.gameObject.layer == LayerMask.NameToLayer("Player 2") && takingDamage) //Players within the trigger while it's not on cooldown will take damage
        {
            other.GetComponent<PlayerHealthManager>().TakeDamage(damage);
            takingDamage = false;
        }
    }
}
