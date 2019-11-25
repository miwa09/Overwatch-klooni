using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdentifier : MonoBehaviour
{
    [Range(1,2)]
    public int player = 1;

    PlayerWeaponMelee playerQuickMelee;
    PlayerMeleeHitRegister quickMeleeHitbox;
    public bool playerHasQuickMelee = false;
    PlayerWeaponRanged playerRangedWeapon;
    public bool playerHasRangedWeapon = false;
    void Start()
    {
        if (playerHasQuickMelee)
        {
            playerQuickMelee = gameObject.GetComponent<PlayerWeaponMelee>();
            quickMeleeHitbox = gameObject.GetComponentInChildren<PlayerMeleeHitRegister>();
        }
        if (playerHasRangedWeapon)
        {
            playerRangedWeapon = gameObject.GetComponent<PlayerWeaponRanged>();
        }
    }

    public void RemoveEnemyFromQuickMelee(GameObject obj)
    {
        quickMeleeHitbox.listHit.Remove(obj);
    }
}
