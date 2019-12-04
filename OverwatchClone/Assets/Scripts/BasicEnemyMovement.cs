using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyMovement : MonoBehaviour
{
    Vector3 origPoint;
    public Transform[] waypoints;
    public float waypointTriggerDistance = 1f;
    int nextWaypoint = 0;
    NavMeshAgent agent;
    bool exploded = false;

    public void GoNextWaypoint() {
        agent.destination = waypoints[nextWaypoint].position;
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GoNextWaypoint();
    }

    void Update()
    {
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

    void Explode() {
        agent.isStopped = true;
        agent.enabled = false;
        Collider[] playersHit = Physics.OverlapSphere(transform.position, 2);
        GameManager gm = FindObjectOfType<GameManager>();
        gm.doorHP -= 40;
        foreach (Collider obj in playersHit) {
            if (obj.tag == "Player") {
                obj.GetComponent<IDamageable>().TakeDamage(20);
            }
        }
        GetComponent<Enemy>().EnemyKill();
        exploded = true;
    }
    public void Death() {
        if (agent.isActiveAndEnabled) {
            agent.isStopped = true;
            agent.enabled = false;
        }
    }
}
