using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    enum EnemyState { Idle, Patrol, Chase }
    enum EnemyType { Walker, Shooter }

    [Header("General Settings")]
    public NavMeshAgent agent;
    [SerializeField] private EnemyType enemyType;

    [SerializeField] private Transform waypoints;
    [SerializeField] private float waitAtPoint = 2f;
    private int currentWaypoint;
    private float waitCounter;

    [SerializeField] private EnemyState enemyState;

    [SerializeField] private float chaseRange;
    [SerializeField] private float suspiciousTime;
    private float timeSeenPlayer;

    [SerializeField] private float bufferDistance;

    [Header("Walker Settings")]
    [SerializeField] private float hitRange = 1.5f;
    [SerializeField] private int damage = 10;

    [Header("Shooter Settings")]
    [SerializeField] private float shootRange = 10f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1f;
    private float fireCooldown;

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
                if (waitCounter > 0)
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
                if (agent.remainingDistance <= 0.1f)
                {
                    currentWaypoint++;
                    if (currentWaypoint >= waypoints.childCount)
                        currentWaypoint = 0;

                    enemyState = EnemyState.Idle;
                    waitCounter = waitAtPoint;
                }

                if (distancetoPlayer <= chaseRange)
                {
                    enemyState = EnemyState.Chase;
                }
                break;

            case EnemyState.Chase:
                if (enemyType == EnemyType.Walker)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.transform.position);

                    if (distancetoPlayer <= hitRange)
                    {
                        Debug.Log("Walker hits player for " + damage);                   
                    }
                }
                else if (enemyType == EnemyType.Shooter)
                {
                    agent.isStopped = true;
                    transform.LookAt(player.transform.position);

                    if (distancetoPlayer <= shootRange)
                    {
                        if (fireCooldown <= 0f)
                        {
                            Shoot();
                            fireCooldown = fireRate;
                        }
                    }
                }

                if (distancetoPlayer > chaseRange)
                {
                    timeSeenPlayer -= Time.deltaTime;

                    if (timeSeenPlayer <= 0)
                    {
                        enemyState = EnemyState.Idle;
                        timeSeenPlayer = suspiciousTime;
                        agent.isStopped = false;
                    }
                }
                else
                {
                    timeSeenPlayer = suspiciousTime;
                }
                break;
        }

        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;
    }

    private void Shoot()
    {
        if (projectilePrefab && firePoint)
        {
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb)
            {
                float bulletSpeed = 20f; 
                rb.AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
            }
        }
    }
}
