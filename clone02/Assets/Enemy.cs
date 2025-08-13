using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    enum EnemyState{ Idle, Patrol, Chase}
    public NavMeshAgent agent;

    [SerializeField] private Transform waypoints;
    [SerializeField] private float waitAtPoint = 2f;//set wait counter
    private int currentWaypoint;
    private float waitCounter;// wait countdown

   [SerializeField] private EnemyState enemyState;

    [SerializeField] private float chaseRange;//chase if player in range

    [SerializeField] private float suspiciousTime;//set time seen
    private float timeSeenPlayer;//time seen countdown

    [SerializeField] private float bufferDistance;

    private GameObject player;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        waitCounter = waitAtPoint;
        timeSeenPlayer = suspiciousTime;
    }

    private void Update()
    {
        float distancetoPlayer = Vector3.Distance(transform.position, player.transform.position);

        switch (enemyState)
        {
            case EnemyState.Idle:
                if(waitCounter > 0)
                {
                    waitCounter -= Time.deltaTime;
                }
                else
                {
                    enemyState = EnemyState.Patrol;
                    agent.SetDestination(waypoints.GetChild(currentWaypoint).position);
                }

                if (distancetoPlayer <= chaseRange)
                {
                    enemyState = EnemyState.Chase;
                }
            break;
            case EnemyState.Patrol:
                if (agent.remainingDistance <= 0.1f)//if close to waypoint
                {
                    currentWaypoint++;
                    if (currentWaypoint >= waypoints.childCount)
                    {
                        currentWaypoint = 0;
                    }
                    enemyState = EnemyState.Idle;
                    waitCounter = waitAtPoint;

                }

                if (distancetoPlayer <= chaseRange)
                {
                    enemyState = EnemyState.Chase;
                }
            break;
            case EnemyState.Chase:
                Vector3 safeDistance = new Vector3(player.transform.position.x+bufferDistance, player.transform.position.y, player.transform.position.z+bufferDistance);//create buffer between player and ai
                agent.SetDestination(safeDistance);
                if (distancetoPlayer > chaseRange);//if far away, stop and observe for player
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    timeSeenPlayer -= Time.deltaTime;

                    if(timeSeenPlayer <= 0)//if couldn't find change to idle
                    {
                        enemyState = EnemyState.Idle;
                        timeSeenPlayer = suspiciousTime;
                        agent.isStopped = false;
                    }
                }

            break;
        }
        
        
 
    }
}
