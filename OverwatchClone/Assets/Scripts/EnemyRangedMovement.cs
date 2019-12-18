using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRangedMovement : MonoBehaviour, Iai
{
    public List<Transform> waypoints;
    public float waypointTriggerDistance = 1f;
    public int nextWaypoint = 0;
    NavMeshAgent agent;
    EnemyRanged gunScript;

    public void GoNextWaypoint() {
        agent.destination = waypoints[nextWaypoint].position;
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        gunScript = GetComponent<EnemyRanged>();
        GoNextWaypoint();
    }

    void Update()
    {
        var d = Vector3.Distance(waypoints[nextWaypoint].position, transform.position);
        if (gunScript.target != null && nextWaypoint > 0) {
            return;
        }

            if (d < waypointTriggerDistance) {
                if (nextWaypoint < waypoints.Count - 1) {
                    nextWaypoint++;
                } else nextWaypoint--;
                GoNextWaypoint();
            }
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
