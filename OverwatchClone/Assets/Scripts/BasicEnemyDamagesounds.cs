using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyDamagesounds : MonoBehaviour
{
    public void PlaySound() {
        int i = Random.Range(1, 3);
        if (i == 1) {
            AudioFW.Play("metal_clang");
        } else AudioFW.Play("metal_clang2");
    }
}
