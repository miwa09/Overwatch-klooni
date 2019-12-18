using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable, IStunable {
    public float hitpoints = 100f;
    public float maxHitpoints = 100f;
    public ParticleSystem deathParticles;
    public bool hasDied = false;
    public bool indestructible = false;
    public float deathDuration = 5f; //How long the dead corpse stays
    public PlayerIdentifier lastDamageSource;
    public Text hpUI;
    public bool roadhogHeal = false;
    public bool isGrounded = false;
    public NavMeshAgent agent;
    public Rigidbody rig;
    public Component[] stunDisableList;
    public bool isBoss = false;
    float stunTimer = 0;
    float stunDuration;
    bool isStunned = false;
    public bool takenDamage = false;


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
            hpUI.text = "" + Mathf.RoundToInt(hitpoints);
        }
        if (hitpoints > maxHitpoints) {
            hitpoints = maxHitpoints;
        }
        if (isStunned) {
            stunTimer += Time.deltaTime;
            if (stunTimer >= stunDuration && !hasDied) {
                EndStun();
            }
        }
    }

    public void EnemyKill()
    {
        hitpoints = 0;
        hpUI.text = "Dead";
        GetComponent<NavMeshAgent>().enabled = false;
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
        GetComponent<Iai>().Death();
        if (isBoss) {
            GameManager gm = (GameManager)FindObjectOfType(typeof(GameManager));
            if (gm.finalStage) {
                gm.bossedDead++;
            }
        }

        hasDied = true; //So the kill function is only run once
    }


    public void DamageSource(PlayerIdentifier player)
    {
        lastDamageSource = player;
    }
    public void TakeDamage(float damage) {
        if (roadhogHeal) {
            hitpoints -= damage / 2;
            return;
        } else {
            hitpoints -= damage;
        }
        if (!isBoss) {
            GetComponent<BasicEnemyDamagesounds>().PlaySound();
        }

    }

    public void Stun(float duration) {
        StunDisableComponents();
        stunTimer = 0;
        stunDuration = duration;
        isStunned = true;
    }

    void StunDisableComponents() {
        if (GetComponent<BasicEnemyMovement>() != null && GetComponent<BasicEnemyMovement>().isWheel) {
            EnemyKill();
            return;
        }
        if (isBoss) {
            if(GetComponent<EnemyBossReaper>() != null) {
                GetComponent<EnemyBossReaper>().stunned = true;
            }
            if (GetComponent<EnemyBossRoadhog>() != null) {
                GetComponent<EnemyBossRoadhog>().stunned = true;
            }
            if (GetComponent<BossJunkrat>() != null) {
                GetComponent<BossJunkrat>().stunned = true;
            }
        }
        agent.isStopped = true;
    }

    void EndStun() {
        isStunned = false;
        stunTimer = 0;
        stunDuration = 0;
        agent.isStopped = false;
        if (GetComponent<BasicEnemyMovement>() != null) {
            GetComponent<BasicEnemyMovement>().GoNextWaypoint();
        }
        if (isBoss) {
            if (GetComponent<EnemyBossReaper>() != null) {
                GetComponent<EnemyBossReaper>().stunned = false;
            }
            if (GetComponent<EnemyBossRoadhog>() != null) {
                GetComponent<EnemyBossRoadhog>().stunned = false;
            }
            if (GetComponent<BossJunkrat>() != null) {
                GetComponent<BossJunkrat>().stunned = false;
            }
        }
    }

    public void DamageKnockback(Vector3 direction, float magnitude, float verticalLaunch) {
        agent.enabled = false;
        rig.isKinematic = false;
        rig.AddForce(Vector3.up * verticalLaunch, ForceMode.Impulse);
        rig.AddForce((direction).normalized * magnitude, ForceMode.Impulse);
    }

    public void Ground() {
        isGrounded = true;
        agent.enabled = true;
        rig.isKinematic = true;
        if (!isStunned && !hasDied && GetComponent<BasicEnemyMovement>() != null) {
            GetComponent<BasicEnemyMovement>().GoNextWaypoint();
        }
    }

}
