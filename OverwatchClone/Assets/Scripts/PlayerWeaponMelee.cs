using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponMelee : MonoBehaviour
{
    public string InputPrefix = "P1"; //Let the script know which player is in question
    public PlayerIdentifier playerIdentifier;
    public BoxCollider meleeCollider; //Get the trigger area for the melee hitbox
    PlayerMeleeHitRegister meleeHit; //Get the component that houses the list of colliders currently within the melee trigger box
    public CameraAnimationManager cameraAnimation; //For camera animations as you melee
    public bool canMelee = true;
    bool cooldown = false;
    float meleeTimer = 0f; //Simple timer
    public float meleeCooldown = 1f;
    public float knockback = 60f; //Adds a little bit of rigidbody knockback on what you hit. This will likely be obsolete sooner than later.
    public float meleeDamage = 30f; //The base melee damage. Default quick melee is 30
    PlayerWeaponRanged playerWeapon;

    //private void OnTriggerEnter(Collider other)
    //{
    //    print("Hit!");
    //    //if (Input.GetButtonDown(InputPrefix + "Melee") && canMelee)
    //    //{
    //        //if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    //        //{
    //        //    other.gameObject.GetComponent<Rigidbody>().AddForce(other.transform.position + Vector3.back * knockback);
    //        //}
    //        //cameraAnimation.CameraMeleeShake();
    //        //canMelee = false;
    //    //}
    //}
    private void Start()
    {
        meleeHit = meleeCollider.GetComponent<PlayerMeleeHitRegister>(); //No unncecessary editor hijinks, just get the list as soon as possible
        playerWeapon = gameObject.GetComponent<PlayerWeaponRanged>();
        playerIdentifier = gameObject.GetComponent<PlayerIdentifier>();
    }

    private void Update()
    {
        if (cooldown) //Turn on the melee cooldown counter
        {
            meleeTimer += Time.deltaTime;
            if (meleeTimer >= meleeCooldown)
            {
                canMelee = true;
                meleeTimer = 0f;
                cooldown = false;
            }
        }
        if (Input.GetButtonDown(InputPrefix + "Melee") && canMelee) //Get the input for melee
        {
            foreach (GameObject enemy in meleeHit.listHit) //Every body collider within the melee's area of effect is hit at once
            {
                enemy.GetComponent<Rigidbody>().AddForce(enemy.transform.position + meleeCollider.transform.forward * knockback);
                var enemyScript = enemy.GetComponent<Enemy>();
                enemyScript.hitpoints -= meleeDamage;
                enemyScript.DamageSource(playerIdentifier, true);
                print(enemyScript.hitpoints + " " + meleeDamage);
            }
            playerWeapon.disabled = true; 
            cameraAnimation.CameraMeleeShake(); //Simple animation
            canMelee = false; //Turn on the cooldown
        }
    }

    public void MeleeDone()
    {
        playerWeapon.disabled = false;
        cooldown = true;
    }
}
