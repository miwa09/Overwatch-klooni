using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemArmor : MonoBehaviour
{
    public float armor = 25f; //How much temporary armor the pickup gives
    public int duration = 5; //For how long the armor is given to the player

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player 1") || other.gameObject.layer == LayerMask.NameToLayer("Player 2")) //Making sure only the players interact with them
        {
            other.gameObject.GetComponent<PlayerHealthManager>().ReceiveTempArmor(armor, duration, 75); //Call the health manager to receive the temporary armor
            GetComponentInParent<ItemSpawn>().itemTaken = true; //So the spawner knows to spawn a new one after a set amount of time (specified in the spawner object itself)
            Destroy(gameObject); //The pickup has done it's job, now it's time for an early retirement
        }
    }
}

