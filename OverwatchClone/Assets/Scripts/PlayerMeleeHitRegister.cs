using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeHitRegister : MonoBehaviour
{
    public List<GameObject> listHit = new List<GameObject>(); //Make a list that adds any enemy colliders touching the melee trigger box

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && other.gameObject.GetComponent<EnemyColliderLocator>().isBody) //Make sure only the body collider goes in, so the enemies dont get hit twice
        {
            listHit.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) //Remove the enemies from the list that have left the trigger
        {
            listHit.Remove(other.gameObject);
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.K)) //For testing purposes
        //{
        //    print(listHit);
        //}
        foreach (GameObject obj in listHit)
        {
            if (obj == null) //If the object is 'killed' while in the trigger box, this lets the list know to remove them from the list, so we don't get any NullReferenceExceptions
            {
                listHit.Remove(obj);
            }
        }
    }
}
