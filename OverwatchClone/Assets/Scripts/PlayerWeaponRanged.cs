using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This is all things that do not need touching. Any changes necessary can be made within the editor. Look at the script all you wish, but do not do any changes within the script itself.
//It's basically running on bubblegum and toothpicks. So don't. Touch. It.
public class PlayerWeaponRanged : MonoBehaviour
{
    public string inputPrefix = "P1"; //What player is being controlled
    public PlayerIdentifier playerIdentifier;

    public float maxDamage = 20; //The damage that occurs before the falloff minimum distance
    public float minDamage = 10; //The damage that occurs after the falloff maximum distance
    public float damageFalloffMinDistance = 30; //At what range does the weapon remain most effective
    public float damageFalloffMaxDistance = 50; //At what range does the weapon become least effective
    float damage; //The damage the weapon inflicts after calculations
    int ammo; //How much ammo the weapon is currently holding
    public int maxAmmo; //The maximum amount of ammo the weapon can hold
    public float shootingCooldown = 0.111f; //How many rounds are fired in a second
    bool canShoot; //To make sure it doesn't fire every frame
    public bool disabled = false;
    float timer = 0f; //Rate of fire stuff
    float reloadTimer = 0f;
    public float reloadTime = 1.55f; //Reload time, will probably be obsolete later
    bool isReloading = false; //So you can't reload during a reload
    public Transform gunOffsetPoint; //What point the gun is actually 'shooting' from, so the center of the camera towards the mouse
    Vector3 gunRayVector;
    public float maxDeviation = 2.4f;
    float currentDeviation;
    RaycastHit hit;
    public Text ammoCount; //UI Element
    int layer1;
    public int layerMask1 = 10; //Which layer we don't want the raycasting to see, it should always be the layer the player is on
    float currentBurst; //Counts how many shots have been fired in a continuous burst
    public ParticleSystem trails;

    private void Start()
    {
        ammo = maxAmmo; //So we don't have to set the ammo count in unity once the script is called upon, it'll always fill the magazine
        playerIdentifier = gameObject.GetComponent<PlayerIdentifier>();
        layer1 = 1 << layerMask1; //Making the layermask cull the selected layer instead
        layer1 = ~layer1;
    }
    void Update()
    {
        //Debug.DrawRay(gunOffsetPoint.position, gunRayVector * 1000f, Color.red); //Testing purposes
        //gunOffset = gunOffsetPoint.position - transform.position;
        //BELOW: Shooting input, shooting cooldown and reloading
        if (disabled)
        {
            isReloading = false;
            reloadTimer = 0f;
        }
        float shoot = Input.GetAxis(inputPrefix + "PrimaryFire"); //Checks how much the right trigger has been pressed
        if (!canShoot && !isReloading)
        {
            timer += Time.deltaTime;
            if (timer >= shootingCooldown) //Throttles the rate of fire
            {
                timer = 0;
                canShoot = true;
                //print(currentBurst + " " + currentDeviation); //Testing purposes
            }
        }
        if (Input.GetButton(inputPrefix + "PrimaryFire") && canShoot && !disabled || shoot > 0.9f && canShoot && !disabled) //If the shoot input is pressed: shoot
        {
            Shoot();
        }
        if (!Input.GetButton(inputPrefix + "PrimaryFire") && shoot < 0.89f)
        {
            currentBurst = 0;
            currentDeviation = 0;
        }
        if (Input.GetButtonDown(inputPrefix + "Reload") && !disabled) //Reload without spending the whole magazine
        {
            isReloading = true;
        }
        if (ammo <= 0) //Automatically reload once ammo runs out
        {
            isReloading = true;
        }
        if (isReloading)
        {
            Reload();
            ammoCount.text = "RELOADING!";
        } else ammoCount.text = ammo + " / " + maxAmmo; //Drawing the ammocount onto the UI
    }

    void Shoot()
    {
        ammo--;
        CalculateAngle();
        currentBurst++;
        //print(ammo); //For testing purposes
        //Testing stuff
        if (Physics.Raycast(gunOffsetPoint.position, gunRayVector, out hit, Mathf.Infinity, layer1)) //Raycast to see what was hit and where
        {
            Collider target = hit.collider; //Naming a bunch of variables
            float distance = hit.distance;
            Vector3 location = hit.point;
            GameObject targetGameObject = hit.collider.gameObject;

            trails.transform.forward = hit.point - trails.transform.position; //Tracer
            trails.Emit(1);

            if (targetGameObject.layer == LayerMask.NameToLayer("Enemy")) //To check that it's an enemy
            {
                if (distance >= damageFalloffMaxDistance) //Adding damage falloff
                {
                    damage = minDamage;
                }
                if (distance <= damageFalloffMinDistance)
                {
                    damage = maxDamage;
                }
                if (distance > damageFalloffMinDistance && distance < damageFalloffMaxDistance)
                {
                    damage = maxDamage * (damageFalloffMinDistance / distance); //A very complicated mathematical formula to deremine the falloff result
                    damage = Mathf.RoundToInt(damage);
                }
                if (targetGameObject.GetComponent<EnemyColliderLocator>().isHead)
                {
                    damage = damage * 2;
                }
                targetGameObject.GetComponentInParent<Enemy>().hitpoints -= damage;
                //targetGameObject.GetComponent<Enemy>().TakeDamage(location - gameObject.transform.position, 150f); //Knockback on hit, was more for fun testing than anything else
                print("Distance: " + distance + " Damage: " + damage + " Hitpoints left: " + targetGameObject.GetComponentInParent<Enemy>().hitpoints);
            }

        }
        canShoot = false;
    }

    void CalculateAngle()
    {
        if (currentBurst < 3) //The first three bullets have no spread
        {
            currentDeviation = 0;
        }
        if (currentBurst >= 3 && currentBurst < 9) //after the third one it grows linearly towars the maximum spread
        {
            currentDeviation = ((currentBurst - 2f) / 6f) * maxDeviation;
        }
        if (currentBurst >= 9) //and after 9 consecutive shots the spread will remain at it's max
        {
            currentDeviation = maxDeviation;
        }
        gunRayVector = Vector3.forward; //creating the spread for the raycast
        float deviation = Random.Range(0f, currentDeviation);
        float angle = Random.Range(0f, 360f);
        gunRayVector = Quaternion.AngleAxis(deviation, Vector3.up) * gunRayVector;
        gunRayVector = Quaternion.AngleAxis(angle, Vector3.forward) * gunRayVector;
        gunRayVector = gunOffsetPoint.rotation * gunRayVector;
    }

    void Reload()
    {
        /*print("Reloading!");*/ //For testing purposes
        canShoot = false;
        reloadTimer += Time.deltaTime;
        if (reloadTimer >= reloadTime && !disabled)
        {
            reloadTimer = 0f;
            ammo = maxAmmo;
            canShoot = true;
            isReloading = false;
            /*print("Reloaded!");*/ //For testing purposes
        }
    }
}
