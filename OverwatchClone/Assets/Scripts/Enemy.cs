﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable {
    public float hitpoints = 100f;
    public float maxHitpoints = 100f;
    public ParticleSystem deathParticles;
    bool hasDied = false;
    bool lastDamageSourceIsMelee = false;
    public bool indestructible = false;
    public float deathDuration = 5f; //How long the dead corpse stays
    Rigidbody rig;
    public PlayerIdentifier lastDamageSource;
    public Text hpUI;

    private void Awake()
    {
        rig = gameObject.GetComponent<Rigidbody>(); //This was added for simple feedback. Will most likely be gone sooner than later.
    }

    void Update()
    {
        if (hitpoints <= 0 && !hasDied) //Destroy the enemy gameobject once it's out of hitpoints
        {
            EnemyKill();
        }
        if (indestructible) //Turn this on and whatever this script is in will be literally unkillable. Use with care. Mostly for testing stuff.
        {
            hitpoints = maxHitpoints;
        }
        if (hpUI != null && !hasDied)
        {
            hpUI.text = "" + hitpoints;
        }
    }

    public void EnemyKill()
    {
        hitpoints = 0;
        hpUI.text = "Dead";
        if (lastDamageSourceIsMelee)
        {
            EnemyDeathCall(true);
        }
        Renderer[] meshes = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in meshes) //This is only so there is some feedback while testing
        {
            rend.material.color = Color.grey;
        }
        Collider[] colliders = gameObject.GetComponentsInChildren<BoxCollider>();
        foreach (Collider col in colliders) //So that players can't hit or collide with dead enemies
        {
            col.gameObject.layer = LayerMask.NameToLayer("Debris");
        }
        deathParticles.Play();
        gameObject.GetComponent<DeathCull>().enabled = true;
        gameObject.GetComponent<BasicEnemyMovement>().Death();
        hasDied = true; //So the kill function is only run once
    }
    public void DamageKnockback(Vector3 knockbackDirection, float knockbackAmount) //If something requires physical knockback
    {
        rig.AddForce(knockbackDirection.normalized * knockbackAmount);
    }

    public void EnemyDeathCall(bool melee)
    {
        if (melee)
        {
            lastDamageSource.RemoveEnemyFromQuickMelee(gameObject);
        }
    }

    public void DamageSource(PlayerIdentifier player)
    {
        lastDamageSource = player;
    }
    public void TakeDamage(float damage) {
        hitpoints -= damage;
    }
}
