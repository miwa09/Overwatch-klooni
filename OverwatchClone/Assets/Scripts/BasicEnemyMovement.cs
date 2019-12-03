using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyMovement : MonoBehaviour
{
    Vector3 origPoint;
    public Transform waypoint;
    NavMeshAgent ai;
    bool reached = false;
    
    void Start()
    {
        origPoint = new Vector3(transform.position.x, 0, transform.position.z);
        ai = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!reached) {
            ai.SetDestination(waypoint.position);
        }
        if (Vector3.Distance(transform.position, waypoint.position) < 1.5f) {
            reached = true;
        }
        if (reached) {
            ai.SetDestination(origPoint);
        }
        if (Vector3.Distance(transform.position, origPoint) < 1.5f) {
            reached = false;
        }
    }
}
