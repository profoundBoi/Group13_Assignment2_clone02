using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform waypoints;
    private int currentWaypoint;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        
        if (agent.remainingDistance <= 0.2f)
        {
            currentWaypoint++;
            if(currentWaypoint >= waypoints.childCount)
            {
                currentWaypoint = 0;
            }
            agent.SetDestination(waypoints.GetChild(currentWaypoint).position);

        }
 
    }

    //private void Observe()
    //{

    //}

    //private void Patrol()
    //{
    //    
    //}
}
