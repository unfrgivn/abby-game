using UnityEngine;

namespace WildsOfCloverhollow.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new Vector3(0f, 15f, -10f);
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private float lookAheadDistance = 2f;
        [SerializeField] private float lookAheadSmoothing = 0.5f;
        
        private Transform target;
        private Vector3 currentLookAhead;
        private Vector3 lastTargetPosition;
        
        private void Start()
        {
            FindPlayer();
        }
        
        private void FindPlayer()
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                lastTargetPosition = target.position;
            }
            else
            {
                Debug.LogWarning("[CameraFollow] No Player found with 'Player' tag.");
            }
        }
        
        private void LateUpdate()
        {
            if (target == null)
            {
                FindPlayer();
                return;
            }
            
            Vector3 movement = target.position - lastTargetPosition;
            lastTargetPosition = target.position;
            
            if (movement.sqrMagnitude > 0.001f)
            {
                Vector3 targetLookAhead = movement.normalized * lookAheadDistance;
                currentLookAhead = Vector3.Lerp(currentLookAhead, targetLookAhead, lookAheadSmoothing * Time.deltaTime * 10f);
            }
            else
            {
                currentLookAhead = Vector3.Lerp(currentLookAhead, Vector3.zero, lookAheadSmoothing * Time.deltaTime * 5f);
            }
            
            Vector3 desiredPosition = target.position + offset + currentLookAhead;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
