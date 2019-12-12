using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossRoadhog : MonoBehaviour
{
    public EnemyBossHook hookScript;
    public EnemyBossRoadhogGun gunScript;
    public Transform target;
    public Enemy baseScript;
    float healTimer = 0;
    float healTicker = 0.01f;
    float healTimer2 = 0;
    float healTicker2 = 1;
    public float healPerTick = 3;
    bool startHealing = false;
    bool isHealing = false;
    float totalHealing = 0;
    public float healCooldown = 8;
    float healCooldownTimer = 0;
    bool healCD = false;
    void Start()
    {
        
    }

    void Update()
    {
        if (target != null) {
            transform.forward = (target.position - transform.position).normalized;
            if (Vector3.Distance(transform.position, target.position) < 10) {
                gunScript.FireWeapon();
            }
        }
        if (baseScript.hitpoints <= baseScript.hitpoints / 2) {
            startHealing = true;
        }
        if (startHealing && !healCD) {
            Heal();
        }
        if (healCD) {
            healCooldownTimer += Time.deltaTime;
            if (healCooldownTimer >= healCooldown) {
                healCooldownTimer = 0;
                healCD = false;
            }
        }
    }

    void Heal() {
        baseScript.roadhogHeal = true;
        if (!isHealing) {
            healTimer2 += Time.deltaTime;
            if (healTimer2 >= healTicker2) {
                isHealing = true;
                healTimer2 = 0;
            }
        }
        if (isHealing) {
            healTimer += Time.deltaTime;
            while (healTimer >= healTicker) {
                healTimer -= healTicker;
                baseScript.hitpoints += healPerTick;
                totalHealing += healPerTick;
                if (totalHealing >= 300) {
                    StopHeal();
                    return;
                }
            }
            healTimer2 += Time.deltaTime;
            if (healTimer2 >= healTicker2) {
                StopHeal();
                return;
            }
        }
    }

    void StopHeal() {
        isHealing = false;
        healTimer = 0;
        healTimer2 = 0;
        totalHealing = 0;
        startHealing = false;
        baseScript.roadhogHeal = false;
        healCD = true;
    }
}
