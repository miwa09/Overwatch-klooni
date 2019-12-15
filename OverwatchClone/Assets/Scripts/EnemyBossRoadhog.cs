using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossRoadhog : MonoBehaviour, Iai
{
    public EnemyBossHook hookScript;
    public EnemyBossRoadhogGun gunScript;
    public Transform target;
    public Enemy baseScript;
    float healTimer = 0;
    float healTicker = 0.01f;
    float healTimer2 = 0;
    float healTicker2 = 1;
    public float targetRange = 25;
    public float healPerTick = 3;
    bool startHealing = false;
    bool isHealing = false;
    float totalHealing = 0;
    public float healCooldown = 8;
    float healCooldownTimer = 0;
    bool healCD = false;
    List<Collider> invisiblePlayers = new List<Collider>();
    List<Collider> playersHit = new List<Collider>();
    public LayerMask groundLayer;
    bool notMoving = true;
    NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        HasTarget();
        if (!HasTarget()) {
            GetTarget();
        }
        if (HasTarget()) {
            CheckTarget();
            if (notMoving) {
                transform.forward = (target.position - transform.position).normalized;
            }
            gunScript.target = target;
            hookScript.target = target;
            GoHook();
            if (Vector3.Distance(transform.position, target.position) < targetRange && !hookScript.isMoving && !baseScript.hasDied) {
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

    void GoHook() {
        if (!hookScript.hookCD) {
            if (TargetDistance() < hookScript.range) {
                hookScript.Hook();
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
    bool HasTarget() {
        if (target != null) {
            return true;
        } else return false;
    }
    void CheckTarget() {
        if (target.GetComponent<PlayerBrigitteShield>() != null) {
            target = target.parent.transform;
        }
        if (target.GetComponent<PlayerHealthManager>().hasDied) {
            target = null;
            playersHit.Clear();
            return;
        }
        if (Vector3.Distance(transform.position, target.position) > targetRange) {
            target = null;
            playersHit.Clear();
            return;
        }
        var direction = target.transform.position - transform.position;
        if (Physics.Raycast(transform.position, direction, targetRange, groundLayer)) {
            target = null;
            playersHit.Clear();
            return;
        }
    }
    void GetTarget() {

        Collider[] hitList = Physics.OverlapSphere(transform.position, targetRange);
        foreach (Collider player in hitList) {
            if (player.tag == "Player") {
                var direction = player.transform.position - transform.position;
                if (Physics.Raycast(transform.position, direction, targetRange, groundLayer)) {
                    if (!invisiblePlayers.Contains(player)) {
                        invisiblePlayers.Add(player);
                    }
                    if (playersHit.Contains(player)) {
                        playersHit.Remove(player);
                    }
                } else if (invisiblePlayers.Contains(player)) {
                    invisiblePlayers.Remove(player);
                }
            }
        }
        foreach (Collider player in hitList) {
            if (!invisiblePlayers.Contains(player) && !playersHit.Contains(player) && player.tag == "Player") {
                playersHit.Add(player);
            }
        }
        if (playersHit.Count == 2) {
            target = MinDistanceTarget(playersHit).transform;
        }
        if (playersHit.Count == 1) {
            target = playersHit[0].transform;
        }
        if (playersHit.Count == 0) {
            target = null;
        }
    }

    Collider MinDistanceTarget(List<Collider> list) {
        var distanceA = Vector3.Distance(transform.position, list[0].transform.position);
        var distanceB = Vector3.Distance(transform.position, list[1].transform.position);
        if (distanceA > distanceB) {
            return list[0];
        } else return list[1];
    }

    float TargetDistance() {
        return Vector3.Distance(transform.position, target.position);
    }
    public void Death() {
        if (agent.isActiveAndEnabled) {
            agent.nextPosition = transform.position;
            agent.enabled = false;
        }
    }
}
