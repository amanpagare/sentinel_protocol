using UnityEngine;

namespace SentinelProtocol
{
    public class EnemySensor : MonoBehaviour
    {
        [SerializeField] private Transform eye;
        [SerializeField] private Transform target;
        [SerializeField] private float viewDistance = 12f;
        [SerializeField, Range(0f, 180f)] private float viewAngle = 100f;
        [SerializeField] private LayerMask obstacleMask = ~0;

        public Transform Target { get => target; set => target = value; }
        public float ViewDistance => viewDistance;
        public float ViewAngle => viewAngle;

        private void Awake()
        {
            if (!eye) eye = transform;
        }

        public bool CanSeeTarget()
        {
            if (!eye || !target) return false;
            Vector3 offset = target.position - eye.position;
            float distance = offset.magnitude;
            if (distance > viewDistance || distance < 0.001f) return false;

            float threshold = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);
            if (Vector3.Dot(eye.forward, offset / distance) < threshold) return false;

            // Only count a ray hit as blocking if it belongs to an obstacle before the target.
            if (Physics.Raycast(eye.position, offset / distance, out RaycastHit hit,
                    distance, obstacleMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform != target && !hit.transform.IsChildOf(target))
                    return false;
            }
            return true;
        }

        private void OnDrawGizmosSelected()
        {
            Transform origin = eye ? eye : transform;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin.position, viewDistance);
            Vector3 left = Quaternion.Euler(0f, -viewAngle * 0.5f, 0f) * origin.forward;
            Vector3 right = Quaternion.Euler(0f, viewAngle * 0.5f, 0f) * origin.forward;
            Gizmos.DrawRay(origin.position, left * viewDistance);
            Gizmos.DrawRay(origin.position, right * viewDistance);
        }
    }
}
