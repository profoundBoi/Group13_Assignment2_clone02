using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    enum EnemyState { Idle, Patrol, Chase }

    [Header("General Settings")]
    public NavMeshAgent agent;
    public Transform player;
    private FirstPersonController playerController; 

    [SerializeField] private Transform waypoints;
    [SerializeField] private float waitAtPoint = 2f;
    [SerializeField] private float chaseRange = 8f;
    [SerializeField] private float fovAngle = 90f;

    private int currentWaypoint;
    private float waitCounter;
    private EnemyState state;

    private MeshRenderer fovRenderer;
    private MeshFilter fovFilter;
    private Mesh fovMesh;

    private void Start()
    {
        if (waypoints != null && waypoints.childCount > 0)
        {
            currentWaypoint = 0;
            agent.SetDestination(waypoints.GetChild(currentWaypoint).position);
            state = EnemyState.Patrol;
        }
        else
        {
            state = EnemyState.Idle;
        }

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player != null)
            playerController = player.GetComponent<FirstPersonController>();

        GameObject fovObject = new GameObject("FOV Mesh");
        fovObject.transform.SetParent(transform);
        fovObject.transform.localPosition = Vector3.zero;
        fovObject.transform.localRotation = Quaternion.identity;

        fovRenderer = fovObject.AddComponent<MeshRenderer>();
        fovRenderer.material = new Material(Shader.Find("Unlit/Color"));
        fovRenderer.material.color = new Color(1f, 1f, 0f, 0.25f); 

        fovFilter = fovObject.AddComponent<MeshFilter>();
        fovMesh = new Mesh();
        fovFilter.mesh = fovMesh;
    }

    private void Update()
    {
        float distancetoPlayer = Vector3.Distance(transform.position, player.position);

        float effectiveChaseRange = chaseRange;
        if (playerController != null && playerController.IsSneaking)
        {
            effectiveChaseRange *= 0.5f;
        }

        switch (state)
        {
            case EnemyState.Idle:
                if (distancetoPlayer <= effectiveChaseRange)
                {
                    state = EnemyState.Chase;
                }
                break;

            case EnemyState.Patrol:
                if (!agent.pathPending && agent.remainingDistance < 0.2f)
                {
                    waitCounter += Time.deltaTime;
                    if (waitCounter >= waitAtPoint)
                    {
                        currentWaypoint = (currentWaypoint + 1) % waypoints.childCount;
                        agent.SetDestination(waypoints.GetChild(currentWaypoint).position);
                        waitCounter = 0f;
                    }
                }

                if (distancetoPlayer <= effectiveChaseRange)
                {
                    state = EnemyState.Chase;
                }
                break;

            case EnemyState.Chase:
                agent.SetDestination(player.position);
                if (distancetoPlayer > effectiveChaseRange)
                {
                    state = EnemyState.Patrol;
                    agent.SetDestination(waypoints.GetChild(currentWaypoint).position);
                }
                break;
        }

        UpdateFOVVisual(effectiveChaseRange);
    }

    private void UpdateFOVVisual(float range)
    {
        int segments = 20;
        float angleStep = fovAngle / segments;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        vertices.Add(Vector3.zero);

        for (int i = 0; i <= segments; i++)
        {
            float angle = (-fovAngle / 2) + (i * angleStep);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
            vertices.Add(dir * range);
        }

        for (int i = 1; i <= segments; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        fovMesh.Clear();
        fovMesh.vertices = vertices.ToArray();
        fovMesh.triangles = triangles.ToArray();
        fovMesh.RecalculateNormals();

        fovRenderer.transform.localPosition = Vector3.zero;
        fovRenderer.transform.localRotation = Quaternion.identity;
    }
}
