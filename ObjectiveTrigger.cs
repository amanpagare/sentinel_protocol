using UnityEngine;

namespace SentinelProtocol
{
    [RequireComponent(typeof(Collider))]
    public class ObjectiveTrigger : MonoBehaviour
    {
        private void Reset() => GetComponent<Collider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<PlayerController>())
                GameManager.Instance?.Win();
        }
    }
}
