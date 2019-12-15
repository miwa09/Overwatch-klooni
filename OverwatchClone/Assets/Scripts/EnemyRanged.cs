using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRanged : MonoBehaviour
{
    public GameObject projectile;
    public Transform target;
    public float attackRange = 40;
    float timer = 0;
    float ticker = 4;
    bool attackCD = true;
    NavMeshAgent ai;
    public LayerMask groundLayer;
    List<Collider> playersHit = new List<Collider>();
    List<Collider> invisiblePlayers = new List<Collider>();

    private void Start() {
        ai = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        HasTarget();
        if (HasTarget()) {
            transform.forward = (target.position - transform.position).normalized;
            ai.isStopped = true;
            if (!attackCD) {
                ShootProjectile();
            }
            CheckTarget();
        } if (!HasTarget()) {
            GetTarget();
            ai.isStopped = false;
        }
        if (attackCD && HasTarget()) {
            timer += Time.deltaTime;
            if (timer >= ticker) {
                timer = 0;
                attackCD = false;
            }
        }
    }

    void ShootProjectile() {
        var rangedAttack = Instantiate(projectile, transform.position, transform.rotation);
        rangedAttack.GetComponent<EnemyRangedAttack>().target = target;
        attackCD = true;
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
        if (Vector3.Distance(transform.position, target.position) > attackRange) {
            target = null;
            playersHit.Clear();
            return;
        }
        var direction = target.transform.position - transform.position;
        if (Physics.Raycast(transform.position, direction, attackRange, groundLayer)) {
            target = null;
            playersHit.Clear();
            return;
        }
    }
    void GetTarget() {

        Collider[] hitList = Physics.OverlapSphere(transform.position, attackRange);
        foreach(Collider player in hitList) {
            if (player.tag == "Player") {
                var direction = player.transform.position - transform.position;
                if (Physics.Raycast(transform.position, direction, attackRange, groundLayer)) {
                    if (!invisiblePlayers.Contains(player)) {
                        invisiblePlayers.Add(player);
                    }
                    if (playersHit.Contains(player)){
                        playersHit.Remove(player);
                    }
                } else if (invisiblePlayers.Contains(player)) {
                    invisiblePlayers.Remove(player);
                }
            }
        }
        foreach(Collider player in hitList) {
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
}
