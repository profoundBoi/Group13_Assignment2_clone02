using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform Player;

    public LayerMask isGround, isPlayer;

    public Vector3 walkPoint;
    private bool setWalkPoint;
    public float walkPointRange;

    public float attackCooldown;
    private bool alreadyAttacking;

    public float sightRange, attackRange;
    public bool playerInSight, playerInRange;

    private void Awake()
    {
        Player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, isPlayer);
        playerInRange = Physics.CheckSphere(transform.position, attackRange, isPlayer);

        if (!playerInSight && !playerInRange)
        {
            Patrol();
        }
        if (playerInSight && !playerInRange)
        {
            Chase();
        }
        if (playerInSight && playerInRange)
        {
            Attack();
        }
    }

    private void Observe()
    {

    }

    private void Patrol()
    {
        if (!setWalkPoint) searchWalkPoint();

        if (setWalkPoint)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalk = transform.position - walkPoint;

        if (distanceToWalk.magnitude < 1f)
        {
            setWalkPoint = false;
        }
    }

    private void searchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        //float randomY = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, isGround))
        {
            setWalkPoint = true;
        }


    }
    private void Chase()
    {
        agent.SetDestination(Player.position);

    }

    private void Attack()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(Player);

        if (!alreadyAttacking)
        {

            alreadyAttacking = true;
            Invoke(nameof(resetAttack), attackCooldown);
        }
    }

    private void resetAttack()
    {
        alreadyAttacking = false;
    }
}
