using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponRanged : MonoBehaviour
{
    public string inputPrefix = "P1"; //What player is being controller

    public float damage = 25f;
    int ammo;
    public int maxAmmo;
    public float shootingCooldown = 0.2f;
    bool canShoot;
    float timer = 0f;
    float reloadTimer = 0f;
    public float reloadTime = 1.5f;
    bool isReloading = false;
    public Camera playerCamera;
    RaycastHit hit;
    Ray ray;

    private void Start()
    {
         ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        ammo = maxAmmo; //So we don't have to set the ammo count in unity once the script is called upon, it'll always fill the magazine
    }
    void Update()
    {

        //BELOW: Shooting input, shooting cooldown and reloading

        float shoot = Input.GetAxis(inputPrefix + "PrimaryFire"); //Checks how much the right trigger has been pressed
        if (!canShoot && !isReloading)
        {
            timer += Time.deltaTime;
            if (timer >= shootingCooldown) //Throttles the rate of fire
            {
                timer = 0;
                canShoot = true;
            }
        }
        if (Input.GetButton(inputPrefix + "PrimaryFire") && canShoot || shoot > 0.9f && canShoot) //If the shoot input is pressed: shoot
        {
            Shoot();
        }
        if (Input.GetButtonDown(inputPrefix + "Reload")) //Reload without spending the whole magazine
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
        }
    }

    void Shoot()
    {
        ammo--;
        print(ammo); //For testing purposes
        Debug.DrawRay(transform.position, ray.direction, Color.red);
        canShoot = false;
    }

    void Reload()
    {
        print("Reloading!"); //For testing purposes
        canShoot = false;
        reloadTimer += Time.deltaTime;
        if (reloadTimer >= reloadTime)
        {
            reloadTimer = 0f;
            ammo = maxAmmo;
            canShoot = true;
            isReloading = false;
            print("Reloaded!"); //For testing purposes
        }
    }
}
