using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyMovement : MonoBehaviour, Iai
{
    public Transform[] waypoints;
    public float waypointTriggerDistance = 1f;
    int nextWaypoint = 0;
    NavMeshAgent agent;
    bool exploded = false;
    float explodeTimer = 0;
    float explodeTicker = 1;
    public float doorDamage = 40;
    public float playerDamage = 20;

    public void GoNextWaypoint() {
        if (waypoints.Length < 1) {
            agent.destination = transform.position;
        }
        agent.destination = waypoints[nextWaypoint].position;
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GoNextWaypoint();
    }

    void Update()
    {
        if (waypoints.Length > 0) {
            var d = Vector3.Distance(transform.position, waypoints[nextWaypoint].position);
            if (d < waypointTriggerDistance) {
                if (nextWaypoint + 1 > waypoints.Length - 1 && !exploded) {
                    Explode();
                } else if (!exploded) {
                    nextWaypoint++;
                    GoNextWaypoint();
                }
            }
        }
    }

    void Explode() {
        agent.destination = transform.position;
            if (!exploded && !GetComponent<Enemy>().hasDied) {
            explodeTimer += Time.deltaTime;
            if (explodeTimer >= explodeTicker) {
                Collider[] playersHit = Physics.OverlapSphere(transform.position, 2);
                GameManager gm = FindObjectOfType<GameManager>();
                gm.doorHP -= doorDamage;
                foreach (Collider obj in playersHit) {
                    if (obj.tag == "Player") {
                        obj.GetComponent<IDamageable>().TakeDamage(playerDamage);
                    }
                }
                GetComponent<Enemy>().EnemyKill();
                agent.enabled = false;
                exploded = true;
            }
        }
    }
    public void Death() {
        if (agent.isActiveAndEnabled) {
            agent.nextPosition = transform.position;
            agent.enabled = false;
        }
    }
}
