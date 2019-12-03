using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTrigger : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    private void OnTriggerEnter(Collider other) {
        other.gameObject.transform.position = player1.position;
    }


}
