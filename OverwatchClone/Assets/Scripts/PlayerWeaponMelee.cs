using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponMelee : MonoBehaviour
{
    public string inputPrefix = "P1"; //Let the script know which player is in question
    public LayerMask enemyLayer;
    public PlayerIdentifier playerIdentifier;
    public Transform playerCamera;
    public CameraAnimationManager cameraAnimation; //For camera animations as you melee
    public bool canMelee = true;
    bool cooldown = false;
    float meleeTimer = 0f; //Simple timer
    public float meleeCooldown = 1f;
    public float meleeRange = 2.5f;
    public float knockback = 2f; //Adds a little bit of rigidbody knockback on what you hit. This will likely be obsolete sooner than later.
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
        playerWeapon = gameObject.GetComponent<PlayerWeaponRanged>();
        playerIdentifier = gameObject.GetComponent<PlayerIdentifier>();
    }

    private void Update()
    {
        Debug.DrawLine(playerCamera.position + playerCamera.forward, playerCamera.position + playerCamera.forward * meleeRange);
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
        if (Input.GetButtonDown(inputPrefix + "Melee") && canMelee) //Get the input for melee
        {
            Vector3 centerpoint = (playerCamera.position + playerCamera.forward * (meleeRange / 2));
            Vector3 size = new Vector3(meleeRange / 2, meleeRange / 2, meleeRange / 2);
            Collider[] hitList = Physics.OverlapBox(centerpoint, size, playerCamera.rotation, enemyLayer);
            foreach (Collider enemy in hitList) {
                if (enemy.GetComponent<EnemyColliderLocator>().isBody) {
                    enemy.GetComponentInParent<IDamageable>().TakeDamage(meleeDamage);
                    enemy.GetComponentInParent<IStunable>().DamageKnockback(enemy.transform.position - playerCamera.position, knockback, 0.4f);
                    GetComponent<IUltCharge>().AddUltCharge(meleeDamage);
                }
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
