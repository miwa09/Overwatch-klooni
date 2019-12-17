using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossJunkrat : MonoBehaviour, Iai
{
    public Transform target;
    public Enemy baseScript;
    List<Collider> playersHit = new List<Collider>();
    List<Collider> invisiblePlayers = new List<Collider>();
    public float targetRange = Mathf.Infinity;
    public LayerMask groundLayer;
    int ammo;
    int maxAmmo = 5;
    public float reloadTime = 1.55f;
    float reloadTimer = 0;
    bool canShoot;
    public float fireRate = 1f;
    float fireRateTimer = 0;
    public GameObject projectile;
    NavMeshAgent agent;
    public bool stunned = false;

    //Movement
    public List<Transform> waypoints;
    int nextWayPoint = 0;
    bool loopingBack = false;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        if (stunned) {
            target = null;
        }
        if (!stunned) {
            if (!HasTarget()) {
                GetTarget();
                MoveAround();
            }
            if (HasTarget()) {
                CheckTarget();
                if (target != null) {
                    transform.forward = (target.position - transform.position).normalized;
                }
                if (ammo > 0 && canShoot && !baseScript.hasDied) {
                    FireWeapon();
                }
            }
            if (ammo <= 0) {
                Reload();
            }
            if (!canShoot) {
                fireRateTimer += Time.deltaTime;
                if (fireRateTimer >= fireRate) {
                    fireRateTimer -= fireRate;
                    canShoot = true;
                }
            }
        }
    }

    void MoveAround() {
        print(nextWayPoint);
        if (waypoints.Count > 0) {
            agent.destination = waypoints[nextWayPoint].position;
            if (Vector3.Distance(transform.position, waypoints[nextWayPoint].position) < 1) {
                if (nextWayPoint < waypoints.Count && !loopingBack) {
                    nextWayPoint++;
                }
                if (nextWayPoint == waypoints.Count) {
                    loopingBack = true;
                }
                if (loopingBack && nextWayPoint > 0) {
                    nextWayPoint--;
                }
                if (loopingBack && nextWayPoint == 0) {
                    loopingBack = false;
                }
            }
        }
    }

    void FireWeapon() {
        ammo--;
        var rangedAttack = Instantiate(projectile, transform.position, transform.rotation);
        rangedAttack.GetComponent<BossJunkratBomb>().target = target;
        canShoot = false;
    }

    void Reload() {
        reloadTimer += Time.deltaTime;
        if (reloadTimer >= reloadTime) {
            reloadTimer = 0;
            ammo = maxAmmo;
            return;
        }
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
        for (int i = 0; i < addedWaypoints.Count; i++) {
            waypoints.Add(addedWaypoints[i]);
        }
    }
}
