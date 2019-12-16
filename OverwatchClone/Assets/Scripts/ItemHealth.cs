using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHealth : MonoBehaviour
{
    public float health = 25f; //How much health the pickup gives

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player 1") || other.gameObject.layer == LayerMask.NameToLayer("Player 2")) //Making sure only players can interact with them
        {
            if (other.gameObject.GetComponent<PlayerHealthManager>().health < other.gameObject.GetComponent<PlayerHealthManager>().maxHealth) //Making sure you can't pick it up with full HP
            {
                other.gameObject.GetComponent<PlayerHealthManager>().ReceiveHealth(health, null); //Calling to the health manager to receive the healing amount specified above
                GetComponentInParent<ItemSpawn>().itemTaken = true; //A call out to the item spawner, so it knows to spawn a new one after a set amount of time (specified in the individual spawners)
                Destroy(gameObject); //Destroy the object, as it has served it's purpose.
            }
        }
    }
}
