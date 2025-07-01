using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private Transform[] waypoint;
    private int waypointIndex;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.avoidancePriority = Mathf.RoundToInt(agent.speed * 10);
    }

    private void Start()
    {
        waypoint = WaypointManager.Instance.GetWaypoints();
    }

    private void Update()
    {
        FaceTarget(agent.steeringTarget);

        if (agent.remainingDistance < .5f)
        {
            agent.SetDestination(GetNextWaypoint());
        }
    }

    private void FaceTarget(Vector3 target)
    {
        Vector3 directionToTarget = target - transform.position;
        if (directionToTarget.magnitude == 0)
            return;

        directionToTarget.y = 0;

        Quaternion newRotation = Quaternion.LookRotation(directionToTarget);

        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, turnSpeed * Time.deltaTime);
    }

    private Vector3 GetNextWaypoint()
    {
        if (waypointIndex >= waypoint.Length)
            return transform.position;
        Vector3 targertPoint = waypoint[waypointIndex].position;
        waypointIndex++;
        return targertPoint;

    }
}
