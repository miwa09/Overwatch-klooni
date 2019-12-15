using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBrigitteShield : MonoBehaviour, IDamageable
{
    public PlayerAbilitiesBrigitte abilityScript;
    public bool isShield;
    public void TakeDamage(float damage) {
        abilityScript.shieldHealth -= damage;
    }
}
