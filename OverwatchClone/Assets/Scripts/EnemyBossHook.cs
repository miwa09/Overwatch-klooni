﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossHook : MonoBehaviour
{
    public float speed = 40;
    public float range = 20;
    public float damage = 30;
    public bool isMoving = false;
    bool isReturning = false;
    Collider hookedPlayer;
    Vector3 targetPos;
    bool hasHooked = false;
    public bool hookCD = false;
    public Transform target;
    public float hookCooldown = 15;
    float hookTimer = 0;
    float afterHookTimer = 0;
    float afterHookTicker = 0.5f;
    bool releaseOnce = true;
    
    

    void Update()
    {
        if (isMoving) {
            if (!isReturning) {
                transform.position += ((targetPos + Vector3.up) - transform.parent.transform.position).normalized * speed * Time.deltaTime;
                if (Vector3.Distance(transform.parent.transform.position, transform.position) > range) {
                    isReturning = true;
                }
            }
            if (isReturning) {
                transform.position += (transform.parent.transform.position - transform.position).normalized * speed * Time.deltaTime;
                if (Vector3.Distance(transform.parent.transform.position, transform.position) < 1.5) {
                    afterHookTimer += Time.deltaTime;
                    if (afterHookTimer >= afterHookTicker) {
                        afterHookTimer = 0;
                        isMoving = false;
                        isReturning = false;
                        releaseOnce = true;
                    }
                    if (releaseOnce) {
                        ReturnToNormal();
                        releaseOnce = false;
                    }
                }
            }
        }
        if (hasHooked = true && hookedPlayer != null) {
            DisableHookedPlayer(hookedPlayer);
            hookedPlayer.transform.position = transform.position;
        }
        if (hookCD) {
            hookTimer += Time.deltaTime;
            if (hookTimer >= hookCooldown) {
                hookTimer = 0;
                hookCD = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.G)) { //Testing purposes
            Hook();
            hookCD = false;
            hookTimer = 0;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && !isReturning) {
            isReturning = true;
            hookedPlayer = other;
            hasHooked = true;
            other.GetComponent<IDamageable>().TakeDamage(damage);
            return;
        }
    }

    public void Hook() {
        targetPos = target.position;
        if (!hookCD && target != null) {
            isMoving = true;
        }
    }

    void DisableHookedPlayer(Collider player) {
        if (player.GetComponent<PlayerIdentifier>().player == 1) {
            player.GetComponent<PlayerWeaponRanged>().enabled = false;
            player.GetComponent<PlayerWeaponMelee>().enabled = false;
            player.GetComponent<PlayerAbilitiesSoldier76>().enabled = false;
        }
        if (player.GetComponent<PlayerIdentifier>().player == 2) {
            player.GetComponentInChildren<PlayerBrigitteMelee>().enabled = false;
            player.GetComponent<PlayerAbilitiesBrigitte>().enabled = false;
        }
        player.GetComponent<PlayerMover>().enabled = false;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().useGravity = false;
    }

    void ActivateHookedPlayer(Collider player) {
        if (player.GetComponent<PlayerIdentifier>().player == 1) {
            player.GetComponent<PlayerWeaponRanged>().enabled = true;
            player.GetComponent<PlayerWeaponMelee>().enabled = true;
            player.GetComponent<PlayerAbilitiesSoldier76>().enabled = true;
        }
        if (player.GetComponent<PlayerIdentifier>().player == 2) {
            player.GetComponentInChildren<PlayerBrigitteMelee>().enabled = true;
            player.GetComponent<PlayerAbilitiesBrigitte>().enabled = true;
        }
        player.GetComponent<PlayerMover>().enabled = true;
        player.GetComponent<Rigidbody>().useGravity = true;
    }

    void ReturnToNormal() {
        transform.position = transform.parent.transform.position;
        hookCD = true;
        if (hookedPlayer != null && hasHooked) {
            hasHooked = false;
            ActivateHookedPlayer(hookedPlayer);
            hookedPlayer = null;
        }
    }
}
