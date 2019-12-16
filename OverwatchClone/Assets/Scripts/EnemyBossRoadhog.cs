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
    public float targetRange = 50;
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
    public Transform[] hookpoints;
    public bool stunned = false;

    //Moving Forward
    public List<Transform> waypoints;
    int nextForwardWaypoint = 0;

    //Getting close
    public float distanceToPlayer = 12;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (stunned) {
            target = null;
        }
        if (!stunned) {
            if (!HasTarget()) {
                GetTarget();
                MoveForward();
            }
            if (HasTarget()) {
                CheckTarget();
                FaceTarget();
                gunScript.target = target;
                hookScript.target = target;
                if (hookScript.hookCD) {
                    GetCloseToTarget();
                }
                if (!hookScript.hookCD) {
                    GoHook();
                }
                if (HasTarget()) {
                    if (Vector3.Distance(transform.position, target.position) < targetRange / 2 && !hookScript.isMoving && !baseScript.hasDied) {
                        gunScript.FireWeapon();
                    }
                }
            }
            BehaviourHeal();
        }
    }

    //Behaviour

    void GetCloseToTarget() {
        if (HasTarget()) {
            agent.destination = target.position;
            if (TargetDistance() < distanceToPlayer) {
                agent.destination = transform.position;
            }
        }
    }

    void BehaviourHeal() {
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
    void MoveForward() {
        if (!HasTarget()) {
            agent.destination = waypoints[nextForwardWaypoint].position;
            if (Vector3.Distance(transform.position, waypoints[nextForwardWaypoint].position) < 1 && nextForwardWaypoint < waypoints.Count) {
                nextForwardWaypoint++;
            }
        }
    }

    void FaceTarget() {
        if (HasTarget()) {
            transform.forward = (target.position - transform.position).normalized;
        }
    }

    void GoHook() {
        if (TargetDistance() < hookScript.range && Vector3.Distance(ClosestHookpoint().position, transform.position) < 1) {
           hookScript.Hook();
        } else {
           agent.destination = ClosestHookpoint().position;
        }
    }

    Transform ClosestHookpoint() {
        int closestPointIndex = 0;
        if (HasTarget()) {
            for (int i = 0; i < hookpoints.Length - 1; i++) {
                var distanceA = Vector3.Distance(target.position, hookpoints[i].position);
                var distanceB = Vector3.Distance(target.position, hookpoints[i + 1].position);
                if (distanceA > distanceB) {
                    closestPointIndex = i + 1;
                }
                closestPointIndex = i;
            }
        }
        return hookpoints[closestPointIndex];
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
            return;
        }
        if (target.GetComponent<PlayerHealthManager>().hasDied) {
            target = null;
            playersHit.Clear();
            GetTarget();
            return;
        }
        if (Vector3.Distance(transform.position, target.position) > targetRange) {
            target = null;
            playersHit.Clear();
            GetTarget();
            return;
        }
        var direction = target.transform.position - transform.position;
        if (Physics.Raycast(transform.position, direction, direction.magnitude, groundLayer)) {
            target = null;
            playersHit.Clear();
            GetTarget();
            return;
        }
    }
    void GetTarget() {

        Collider[] hitList = Physics.OverlapSphere(transform.position, targetRange);
        foreach (Collider player in hitList) {
            if (player.tag == "Player") {
                var direction = player.transform.position - transform.position;
                if (Physics.Raycast(transform.position, direction, direction.magnitude, groundLayer)) {
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
            print("found two");
            target = MinDistanceTarget(playersHit).transform;
            return;
        }
        if (playersHit.Count == 1) {
            target = playersHit[0].transform;
            return;
        }
        if (playersHit.Count == 0) {
            target = null;
            return;
        }
    }

    Collider MinDistanceTarget(List<Collider> list) {
        var distanceA = Vector3.Distance(transform.position, list[0].transform.position);
        var distanceB = Vector3.Distance(transform.position, list[1].transform.position);
        if (distanceA < distanceB) {
            return list[0];
        } else return list[1];
    }

    float TargetDistance() {
        if (HasTarget()) {
            return Vector3.Distance(transform.position, target.position);
        } else return Mathf.Infinity;
    }
    public void Death() {
        if (agent.isActiveAndEnabled) {
            agent.isStopped = false;
            agent.nextPosition = transform.position;
            agent.enabled = false;
        }
    }
    public void AddWaypoints(List<Transform> addedWaypoints) {
        waypoints = addedWaypoints;
    }
}
