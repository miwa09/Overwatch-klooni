using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BasicEnemyMovement : MonoBehaviour, Iai, IStoppable
{
    public List<Transform> waypoints;
    public float waypointTriggerDistance = 1f;
    int nextWaypoint = 0;
    NavMeshAgent agent;
    bool exploded = false;
    float explodeTimer = 0;
    public float explodeAfter = 1;
    public float doorDamage = 40;
    public float playerDamage = 20;
    float stopTimer = 0;
    public float stopTime = 0.3f;
    bool isStopped = false;
    public bool isWheel = false;


    public void GoNextWaypoint() {
        if (waypoints.Count < 1) {
            agent.destination = transform.position;
        }
        agent.destination = waypoints[nextWaypoint].position;
    }
    void Start()
    {
        if (isWheel) {
            AudioFW.Play("warning");
        }
        agent = GetComponent<NavMeshAgent>();
        GoNextWaypoint();
    }

    void Update()
    {
        if (waypoints.Count > 0) {
            var d = Vector3.Distance(transform.position, waypoints[nextWaypoint].position);
            if (d < waypointTriggerDistance) {
                if (nextWaypoint + 1 > waypoints.Count - 1 && !exploded) {
                    Explode();
                } else if (!exploded) {
                    nextWaypoint++;
                    GoNextWaypoint();
                }
            }
        }
        if (isStopped) {
            Stopped();
        }
    }

    void Explode() {
        agent.destination = transform.position;
            if (!exploded && !GetComponent<Enemy>().hasDied) {
            explodeTimer += Time.deltaTime;
            if (explodeTimer >= explodeAfter) {
                Collider[] playersHit = Physics.OverlapSphere(transform.position, 2);
                GameManager gm = FindObjectOfType<GameManager>();
                gm.doorHP -= doorDamage;
                AudioFW.Play("explosion");
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

    public void StopMovement() {
        if (isWheel) {
            return;
        }
        isStopped = true;
        agent.isStopped = true;
    }

    void Stopped() {
        stopTimer += Time.deltaTime;
        if (stopTimer >= stopTime) {
            stopTimer = 0;
            isStopped = false;
            agent.isStopped = false;
            return;
        }
    }

    public void AddWaypoints(List<Transform> addedWaypoints) {
        waypoints = addedWaypoints;
    }
}
