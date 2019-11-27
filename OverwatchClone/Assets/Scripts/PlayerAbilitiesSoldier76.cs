using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilitiesSoldier76 : MonoBehaviour
{
    public string inputPrefix = "P1";
    public PlayerIdentifier playerIdentifier;

    public float sprintSpeedMultiplier = 1.5f;
    float baseSpeedMultiplier;
    public bool canRun = true;
    public bool isRunning = false;
    bool endingRun = false;
    PlayerMovementScript moveScript;
    PlayerWeaponRanged gunScript;
    public GameObject healAbilityPrefab;
    bool ability3CooldownOn = false;
    float ability3CooldownTimer = 0;
    float ability3CooldownTicker = 1f;
    public int healCooldown = 15;
    int healCooldownCounter = 0;
    public Text healUI;
    public GameObject rocketAbilityPrefab;
    public GameObject cameraLook;
    bool ability1CooldownOn = false;
    float ability1CooldownTimer = 0;
    float ability1CooldownTicker = 1f;
    public int rocketCooldown = 8;
    int rocketCooldownCounter = 0;
    public Text rocketUI;
    
    void Start()
    {
        playerIdentifier = gameObject.GetComponent<PlayerIdentifier>();
        moveScript = gameObject.GetComponent<PlayerMovementScript>();
        gunScript = gameObject.GetComponent<PlayerWeaponRanged>();
        baseSpeedMultiplier = sprintSpeedMultiplier;
    }

    private void Update()
    {
        if (canRun)
        {
            SAbilitySprint();
        }
        if (isRunning && !canRun)
        {
            SprintEndBuffer();
        }
        if (ability3CooldownOn)
        {
            HealCooldown();
            healUI.text = "Heal " + healCooldownCounter;
            healUI.color = Color.red;
        }
        if (!ability3CooldownOn)
        {
            healUI.text = "Heal";
            healUI.color = Color.white;
            healCooldownCounter = healCooldown;
            SAbilityHeal();
        }
        if (ability1CooldownOn)
        {
            RocketCooldown();
            rocketUI.text = "Rocket " + rocketCooldownCounter;
            rocketUI.color = Color.red;
        }
        if (!ability1CooldownOn)
        {
            rocketUI.text = "Rocket";
            rocketUI.color = Color.white;
            rocketCooldownCounter = rocketCooldown;
            SAbilityRockets();
        }
    }
    public void SAbilitySprint()
    {
        if (Input.GetButton(inputPrefix + "Ability2") && canRun)
        {
            SprintStart();
        }
        if (isRunning && Input.GetButton(inputPrefix + "PrimaryFire"))
        {
            SprintEndBuffer();
            endingRun = false;
        }
        if (isRunning && Input.GetButtonDown(inputPrefix + "Reload") && gunScript.ammo < gunScript.maxAmmo)
        {
            gunScript.disabled = false;
            gunScript.isReloading = true;
            SprintEnd();
        }
        if (isRunning && !Input.GetButton(inputPrefix + "Ability2"))
        {
            SprintEndBuffer();
            endingRun = true;
        }
    }

    public void SAbilityHeal()
    {
        if (Input.GetButton(inputPrefix + "Ability3"))
        {
            Instantiate(healAbilityPrefab, transform.position, new Quaternion(0, 0, 0, 0));
            ability3CooldownOn = true;
        }
    }

    public void SAbilityRockets()
    {
        if (Input.GetButton(inputPrefix + "Ability1"))
        {
            Instantiate(rocketAbilityPrefab, gunScript.gunOffsetPoint.position, cameraLook.transform.rotation);
            ability1CooldownOn = true;
        }
    }
    public void SAbilityUlti()
    {
        if (Input.GetButton(inputPrefix + "AbilityUlt"))
        {

        }
    }

    void SprintStart()
    {
        if (endingRun)
        {
            sprintSpeedMultiplier = baseSpeedMultiplier;
            endingRun = false;
        }
        if (isRunning)
        {
            Sprint();
            return;
        }
        gunScript.disabled = true;
        isRunning = true;
        sprintSpeedMultiplier = baseSpeedMultiplier;
    }

    void Sprint()
    {
        moveScript.movementSpeed = moveScript.movementSpeed * sprintSpeedMultiplier;
        print(moveScript.movementSpeed);
    }
    void SprintEndBuffer()
    {
        if (endingRun && Input.GetButton(inputPrefix + "Ability2"))
        {
            SprintStart();
            return;
        }
        if (sprintSpeedMultiplier > 1)
        {
            sprintSpeedMultiplier -= 1.67f * Time.deltaTime;
        }
        if (sprintSpeedMultiplier <= 1)
        {
            SprintEnd();
            return;
        }
    }
    void SprintEnd()
    {
        gunScript.disabled = false;
        isRunning = false;
    }

    void HealCooldown()
    {
        ability3CooldownTimer += Time.deltaTime;
        if (ability3CooldownTimer >= ability3CooldownTicker)
        {
            ability3CooldownTimer -= ability3CooldownTicker;
            healCooldownCounter--;
        }
        if (healCooldownCounter <= 0 && ability3CooldownOn)
        {
            ability3CooldownOn = false;
        }
    }
    void RocketCooldown()
    {
        ability1CooldownTimer += Time.deltaTime;
        if (ability1CooldownTimer >= ability1CooldownTicker)
        {
            ability1CooldownTimer -= ability1CooldownTicker;
            rocketCooldownCounter--;
        }
        if (rocketCooldownCounter <= 0 && ability1CooldownOn)
        {
            ability1CooldownOn = false;
        }
    }

    //bool HealCooldownTracker()
    //{
    //    if (healCooldownCounter <= 0 && ability2CooldownOn)
    //    {
    //        ability2CooldownOn = false;
    //        return true;
    //    }
    //    else return false;
    //}
}
