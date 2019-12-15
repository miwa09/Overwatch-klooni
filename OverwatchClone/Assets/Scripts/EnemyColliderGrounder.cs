using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderGrounder : MonoBehaviour
{
    public Enemy baseScript;
    public LayerMask groundLayer;

    private void FixedUpdate() {
        if (Physics.CheckSphere(transform.position, 0.11f, groundLayer)) {
            baseScript.Ground();
        } else baseScript.isGrounded = false;
    }
}
