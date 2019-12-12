
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour, IDamageable {
    public float maxHealth = 100f; //The amount of health the player can never exceed. Also the default amount when the game starts
    public float health;
    float healthBeforeHeal; //If you have base armor, this is so we can know how much armor you need after receiving the health you were missing
    public float tempArmor; //Temporary armor. Given through skills, but only for a short amount of time.
    float maxTempArmor = 75f; //A maximum amount for temporary armor, just so nothing breaks
    //bool hasTempArmor = false; //obsolete
    int tempArmorTicks = 0; //Timed ticks so we can know when to remove the temporary armor.
    int tempArmorMaxTicks; //How long temporary armor lasts
    float baseArmor; //Some characters might have a base amount of armor instead of purely health.
    public float baseArmorMax; //Serves the same purpose as health max
    public float permArmor; //Permanent armor is like temporary armor, it's just removed only by taking enough damage
    float maxPermArmor; //Maximum amount of permanent armor
    float timer = 0f; //Default ticker counting time.
    float ticker = 1f;
    float allArmor; //To make calculacting the armors a bit easier
    public Text hud_health; //Different HUD elements for showing health and the different armors. Mostly for testing purposes.
    public Text hud_baseArmor;
    public Text hud_permArmor;
    public Text hud_tempArmor;
    Vector3 spawnPoint;
    public int respawnTime = 10;
    float respawnTimer = 0;
    float respawnTicker = 1;
    int respawnSecsPassed = 0;
    public Text respawnUI;
    public Text deathText;
    GameManager gameManager;
    bool sendOnce = true;
    public bool hasDied = false;
    bool godMode = false;

    bool hasTempArmor() //Check to see if there is temporary armor in play, so we can remove it when it's duration is up
    {
        if (tempArmor > 0)
        {
            return true;
        }
        else return false;
    }
    void Start() //So we don't have to set the starting stats manually for different characters, just set the max and you're good to go
    {
        health = maxHealth;
        baseArmor = baseArmorMax;
        maxPermArmor = permArmor;
        spawnPoint = transform.position;
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (health >= maxHealth) //The player should never have more than their maximum health. This clamp makes sure of it.
        {
            health = maxHealth;
        }
        health = Mathf.RoundToInt(health); //Nobody wants to see millions of decimals when they're looking for health. Fast and easy integers are pleasing to the eye.
        tempArmor = Mathf.RoundToInt(tempArmor);
        if (permArmor >= maxPermArmor) //Similar clamping to permanent armor as health.
        {
            permArmor = maxPermArmor;
        }
        permArmor = Mathf.RoundToInt(permArmor);
        allArmor = tempArmor + permArmor + baseArmor; //Calculating all the armor for an easier time down below
        timer += Time.deltaTime; //Default timer
        while (timer >= ticker)
        {
            timer -= ticker;
            if (hasTempArmor()) //Temporary armor ticker only begins counting when there's temporary armor active
            {
                tempArmorTicks++;
            }
        }
        if (tempArmorTicks >= tempArmorMaxTicks) //If it's time is up, it's gone.
        {
            tempArmor = 0f;
            tempArmorTicks = 0;
            tempArmorMaxTicks = 5;
        }
        //if (allArmor > 0)
        //{
        //    hud.text = (health + allArmor) + " / " + (maxHealth + baseArmorMax);
        //    hud.color = new Color(237f, 160f, 0f);
        //}
        //else
        //{
        hud_health.text = health + " / " + maxHealth; //Actually drawing the UI elements to say what we want
        if (baseArmorMax > 0)
        {
            hud_baseArmor.text = baseArmor + " / " + baseArmorMax;
        }
        else hud_baseArmor.text = "";
        if (tempArmor > 0)
        {
            hud_tempArmor.text = "" + tempArmor;
        }
        else hud_tempArmor.text = "";
        if (permArmor > 0)
        {
            hud_permArmor.text = "" + permArmor;
        } else hud_permArmor.text = "";

        if (health <= 0) {
            PlayerDeath();
        }
        if (godMode) {
            health = maxHealth;
        }
        if (Input.GetKeyDown(KeyCode.F4)) {
            godMode = true;
        }
        //hud.color = Color.red;
        //}
        //print("Base: " + baseArmor + " Perm: " + permArmor + " Temp: " + tempArmor);
    }



    public void TakeDamage(float damage)
    {
        if (godMode) {
            return;
        }
        //print("BEFORE // Health: " + health + " / Temp Armor: " + tempArmor + " Perm Armor: " + permArmor + " Base Armor: " + baseArmor); //For testing purposes
        if (allArmor > 0)
        {
            if (damage > 6) //Armor works by lesseing the damage by 3 points whenever it's above six.
            {
                damage -= 3;
            }
            else
            {
                damage = damage / 2; //Below six, we simply half the damage.
                damage = Mathf.RoundToInt(damage); //Round it to integers so we don't get nasty decimals mucking about our health and armor
            }

            if (damage > tempArmor) //First we damage the temporary armor, if there is any
            {
                damage -= tempArmor;
                tempArmor = 0;
            } else
            tempArmor -= damage;

            if (damage > permArmor && tempArmor <= 0) //Then we move onto permanent armor
            {
                damage -= permArmor;
                permArmor = 0;
            } else if (tempArmor <= 0)
            permArmor -= damage;

            if (damage > baseArmor && permArmor <= 0 && tempArmor <= 0) //And if there isn't temporary or permanent armor, we go to the base armor.
            {
                damage -= baseArmor;
                baseArmor = 0;
            } else if (tempArmor <= 0 && permArmor <= 0)
            baseArmor -= damage;
        }
        if (tempArmor + permArmor + baseArmor <= 0 && damage > 0) //Lastly the health takes damage, and any surplus from the armors if there was more damage than armor
        {
             health -= damage;
        }
        //print("AFTER // Health: " + health + " / Temp Armor: " + tempArmor + " Perm Armor: " + permArmor + " Base Armor: " + baseArmor); //For testing purposes
    }

    public void ReceiveHealth(float heal) //A function that other scripts can call out to
    {
        healthBeforeHeal = health; //We went over this in the beginning
        health += heal;
        if (health >= maxHealth && baseArmorMax > 0) //if health is at the max already, but the player character has any base armor, that get's healed here, if possible
        {
            health = maxHealth;
            heal = heal - (health - healthBeforeHeal);
            baseArmor += heal;
            if (baseArmor >= baseArmorMax)
            {
                 baseArmor = baseArmorMax;
            }
        }
    }

    public void ReceiveTempArmor(float armor, int duration) //A function that other scripts can call out to give out temporary armor. The new duration repeats the old one if it's not shorter than the current one
    {
        tempArmor += armor;
        if (tempArmor >= maxTempArmor)
        {
            tempArmor = maxTempArmor;
        }
        if (duration > tempArmorTicks)
        {
            tempArmorTicks = 0;
        }
    }

    public void ReceivePermArmor(float armor, float maxArmor) //Same, but with permanenent armor. The maxArmor float increases the character's overall permanent armor maximum, so the maximum is based on what gives the characters the armor
    {
        permArmor += armor;
        maxPermArmor = maxArmor;
    }

    void PlayerDeath() {
        if (sendOnce) {
            gameManager.playersDead ++;
            sendOnce = false;
        }
        hasDied = true;
        health = 0;
        baseArmor = 0;
        permArmor = 0;
        tempArmor = 0;
        if (GetComponent<PlayerIdentifier>().player == 1) {
            GetComponent<PlayerWeaponRanged>().enabled = false;
            GetComponent<PlayerWeaponMelee>().enabled = false;
            GetComponent<PlayerAbilitiesSoldier76>().enabled = false;
        }
        GetComponent<PlayerMover>().enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().useGravity = false;
        respawnTimer += Time.deltaTime;
        deathText.text = "You've died!";
        respawnUI.text = "Respawn in: " + (respawnTime - respawnSecsPassed);
        if (respawnTimer >= respawnTicker) {
            respawnTimer -= respawnTicker;
            respawnSecsPassed++;
        }
        if (respawnSecsPassed >= respawnTime) {
            PlayerRespawn();
        }
    }
    void PlayerRespawn() {
        hasDied = false;
        gameManager.playersDead--;
        sendOnce = true;
        health = maxHealth;
        baseArmor = baseArmorMax;
        deathText.text = "";
        respawnUI.text = "";
        GetComponent<Rigidbody>().position = spawnPoint;
        if (GetComponent<PlayerIdentifier>().player == 1) {
            GetComponent<PlayerWeaponRanged>().enabled = true;
            GetComponent<PlayerWeaponMelee>().enabled = true;
            GetComponent<PlayerAbilitiesSoldier76>().enabled = true;
        }
        GetComponent<PlayerMover>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
    }
}
