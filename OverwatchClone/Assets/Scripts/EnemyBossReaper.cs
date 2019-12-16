using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBossReaper : MonoBehaviour, Iai
{
    public EnemyBossReaperGun gunScript;
    public Transform target;
    public Enemy baseScript;
    public float targetRange = 25;
    public float healPerTick = 3;
    bool isGhost = false;
    public float ghostDuration = 3;
    float ghostDurationTimer = 0;
    public float ghostCooldown = 8;
    float lastHitpoints;
    float ghostCooldownTimer = 0;
    bool ghostCD = false;
    List<Collider> invisiblePlayers = new List<Collider>();
    List<Collider> playersHit = new List<Collider>();
    public LayerMask groundLayer;
    bool notMoving = true;
    public Collider[] hitboxes;
    bool ultOn = false;
    public float ultRadius = 8;
    public float ultDamagerPerHit = 34;
    public int ultDuration = 3;
    float ultTimer = 0;
    float ultTicker = 0.25f;
    int ultTicks = 0;
    int ultSecondsPassed = 0;
    public LayerMask playerLayer;
    NavMeshAgent agent;
    public bool stunned = false;
    public Material normalMaterial;
    public Material ghostMaterial;
    public Renderer meshRenderer;

    bool nextToTarget = false;
    public float distanceToTarget = 8;

    bool hasDirection = false;
    float strafeTimer = 0;
    float strafeTicker = 2;
    int leftOrRight;

    bool canUlt = true;
    bool inUltRange = false;

    List<Transform> waypoints;
    int nextForwardWaypoint = 0;

    void Start()
    {
        lastHitpoints = baseScript.hitpoints;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (stunned) {
            target = null;
            ultOn = false;
        }
        if (!stunned) {
            HasTarget();
            if (!HasTarget()) {
                GetTarget();
            }
            if (!nextToTarget && !ultOn) {
                GetCloseToTarget();
            }
            if (nextToTarget && !ultOn) {
                CheckTargetDistance();
                StrafeTarget();
                if (canUlt) {
                    MoveUltimate();
                }
            }
            if (HasTarget()) {
                CheckTarget();
                if (target != null) {
                    transform.forward = (target.position - transform.position).normalized;
                }
                //if (HasTarget() && gunScript.canShoot && TargetDistance() < targetRange && !isGhost && !ultOn && !baseScript.hasDied) {
                //    gunScript.target = target;
                //    gunScript.FireWeapon();
                //}
            }
            GhostMode();
            if (isGhost) {
                GhostModeStart();
            }
            if (Input.GetKeyDown(KeyCode.M)) {
                ultOn = true;
            }
            if (ultOn) {
                Ultimate();
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

    void GetCloseToTarget() {
        if (target != null) {
            agent.destination = target.position;
            if (Vector3.Distance(target.position, transform.position) < distanceToTarget) {
                nextToTarget = true;
            }
        }
    }

    void StrafeTarget() {
        if (!hasDirection) {
            leftOrRight = Random.Range(1, 3);
            hasDirection = true;
        }
        if (hasDirection && leftOrRight == 2) {
            agent.destination = target.position + ((target.position + Vector3.right).normalized * distanceToTarget);
        }
        if (hasDirection && leftOrRight == 1) {
            agent.destination = target.position + ((target.position + Vector3.left).normalized * distanceToTarget);
        }
        if (hasDirection) {
            strafeTimer += Time.deltaTime;
            if (strafeTimer >= strafeTicker) {
                strafeTimer = 0;
                hasDirection = false;
            }
        }
    }

    void MoveUltimate() {
        if (target != null) {
            agent.destination = target.position;
            if (Vector3.Distance(target.position, transform.position) < 3) {
                if (canUlt) {
                    ultOn = true;
                }
                agent.destination = transform.position;
            }
            if (Vector3.Distance(target.position, transform.position) < ultRadius - 1) {
                inUltRange = true;
            }
            if (ultOn && !inUltRange) {
                agent.destination = transform.position;
            }
        }
    }

    void CheckTargetDistance() {
        if (Vector3.Distance(target.position, transform.position) < distanceToTarget) {
            nextToTarget = true;
        }
        if (Vector3.Distance(target.position, transform.position) > distanceToTarget) {
            nextToTarget = false;
            hasDirection = false;
        }
    }

    void GhostMode() {
        if (baseScript.hitpoints < lastHitpoints && !ghostCD && !isGhost) {
            isGhost = true;
        }
        if (ghostCD && !isGhost) {
            ghostCooldownTimer += Time.deltaTime;
            if (ghostCooldownTimer >= ghostCooldown) {
                ghostCooldownTimer = 0;
                ghostCD = false;
            }
        }
    }
    void GhostModeStart() {
        foreach (Collider hitbox in hitboxes) {
            hitbox.enabled = false;
        }
        meshRenderer.material = ghostMaterial;
        ghostDurationTimer += Time.deltaTime;
        if (ghostDurationTimer >= ghostDuration) {
            ghostDurationTimer = 0;
            GhostModeEnd();
        }
    }
    void GhostModeEnd() {
        isGhost = false;
        ghostCD = true;
        lastHitpoints = baseScript.hitpoints;
        meshRenderer.material = normalMaterial;
        foreach (Collider hitbox in hitboxes) {
            hitbox.enabled = true;
        }
    }

    void Ultimate() {
        ultTimer += Time.deltaTime;
        canUlt = false;
        if (ultTimer >= ultTicker) {
            ultTimer -= ultTicker;
            ultTicks++;
            if (ultTicks >= 4) {
                ultTicks -= 4;
                ultSecondsPassed++;
            }
            if (ultSecondsPassed >= ultDuration) {
                UltEnd();
                return;
            }
            Collider[] ultArea = Physics.OverlapSphere(transform.position, ultRadius, playerLayer);
            print(ultArea.Length);
            foreach(Collider player in ultArea) {
                if (player.tag == "Player") {
                    player.GetComponent<IDamageable>().TakeDamage(ultDamagerPerHit);
                }
            }
        }

    }
    void UltEnd() {
        ultOn = false;
        ultTicks = 0;
        ultSecondsPassed = 0;
        ultTimer = 0;
    }

    public void Lifesteal(float damage) {
        var lifesteal = damage * 0.4f;
        baseScript.hitpoints += lifesteal;
        lastHitpoints = baseScript.hitpoints;
    }

    //Getting and checking the targeted player
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
        if (TargetDistance() > targetRange) {
            target = null;
            playersHit.Clear();
            return;
        }
        var direction = target.transform.position - transform.position;
        if (Physics.Raycast(transform.position, direction, direction.magnitude, groundLayer)) {
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
            agent.isStopped = false;
            agent.nextPosition = transform.position;
            agent.enabled = false;
        }
    }
    public void AddWaypoints(List<Transform> addedWaypoints) {
        waypoints = addedWaypoints;
    }
}
