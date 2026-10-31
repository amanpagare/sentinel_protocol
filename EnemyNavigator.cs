using UnityEngine;
using UnityEngine.AI;

namespace SentinelProtocol
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyNavigator : MonoBehaviour
    {
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float patrolWait = 1f;
        private NavMeshAgent agent;
        private int index;
        private float waitUntil;

        public NavMeshAgent Agent => agent;
        public Vector3 CurrentPatrolPoint =>
            patrolPoints != null && patrolPoints.Length > 0 && patrolPoints[index % patrolPoints.Length]
                ? patrolPoints[index % patrolPoints.Length].position : transform.position;

        private void Awake() => agent = GetComponent<NavMeshAgent>();

        public void SetPatrolPoints(Transform[] points) => patrolPoints = points;

        public void Patrol()
        {
            if (!agent || !agent.isOnNavMesh || patrolPoints == null || patrolPoints.Length == 0) return;
            if (Time.time < waitUntil) return;

            if (!agent.pathPending && (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance + 0.15f))
            {
                waitUntil = Time.time + patrolWait;
                index = (index + 1) % patrolPoints.Length;
                if (patrolPoints[index]) agent.SetDestination(patrolPoints[index].position);
            }
            else if (!agent.hasPath && patrolPoints[index])
            {
                agent.SetDestination(patrolPoints[index].position);
            }
        }

        public void MoveTo(Vector3 position)
        {
            if (agent && agent.isOnNavMesh && (!agent.hasPath || (agent.destination - position).sqrMagnitude > 0.5f))
                agent.SetDestination(position);
        }

        public bool ReachedDestination(float tolerance = 0.6f) =>
            agent && agent.isOnNavMesh && !agent.pathPending &&
            agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, tolerance);
    }
}
