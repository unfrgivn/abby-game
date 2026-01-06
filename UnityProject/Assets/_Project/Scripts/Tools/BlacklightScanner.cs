using UnityEngine;
using WildsOfCloverhollow.Bootstrap;
using WildsOfCloverhollow.Content;
using WildsOfCloverhollow.Core;

namespace WildsOfCloverhollow.Tools
{
    public class BlacklightScanner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LanternTuning tuning;
        [SerializeField] private Transform scanOrigin;
        [SerializeField] private GameObject lanternVisualEffect;
        
        [Header("Layers")]
        [SerializeField] private LayerMask revealableLayer;
        
        [Header("Debug")]
        [SerializeField] private bool debugForceActive;
        
        private bool isActive;
        private float scanTimer;
        private float scanInterval;
        
        private Collider[] scanResults;
        private IBlacklightRevealable currentTarget;
        private float currentProgress;
        
        public bool IsActive => isActive;
        public IBlacklightRevealable CurrentTarget => currentTarget;
        
        public static BlacklightScanner Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            if (tuning != null)
            {
                scanInterval = 1f / tuning.scanFrequency;
                scanResults = new Collider[tuning.maxRevealablesPerScan];
            }
            else
            {
                scanInterval = 1f / 15f;
                scanResults = new Collider[10];
            }
            
            if (scanOrigin == null)
                scanOrigin = transform;
                
            SetLanternVisual(false);
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
        
        private void OnEnable()
        {
            if (InputRouter.Instance != null)
                InputRouter.Instance.OnLantern += HandleLanternInput;
        }
        
        private void OnDisable()
        {
            if (InputRouter.Instance != null)
                InputRouter.Instance.OnLantern -= HandleLanternInput;
        }
        
        private void Update()
        {
            if (debugForceActive && !isActive)
            {
                SetLanternActive(true);
            }
            else if (!debugForceActive && isActive)
            {
                SetLanternActive(false);
            }
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.L))
            {
                debugForceActive = !debugForceActive;
            }
            
            if (!isActive)
            {
                DecayProgress();
                return;
            }
            
            scanTimer += Time.deltaTime;
            if (scanTimer >= scanInterval)
            {
                scanTimer = 0f;
                PerformScan();
            }
        }
        
        private void HandleLanternInput()
        {
            var gameState = GameStateManager.Instance?.State;
            if (gameState != null && !gameState.IsLanternUnlocked)
            {
                Debug.Log("[BlacklightScanner] Lantern not unlocked yet.");
                return;
            }
            
            ToggleLantern();
        }
        
        public void ToggleLantern()
        {
            isActive = !isActive;
            SetLanternVisual(isActive);
            
            if (!isActive && currentTarget != null)
            {
                currentTarget.OnRevealInterrupted();
                currentTarget = null;
                currentProgress = 0f;
            }
            
            Debug.Log($"[BlacklightScanner] Lantern {(isActive ? "ON" : "OFF")}");
        }
        
        public void SetLanternActive(bool active)
        {
            if (isActive == active)
                return;
                
            isActive = active;
            SetLanternVisual(active);
            
            if (!active && currentTarget != null)
            {
                currentTarget.OnRevealInterrupted();
                currentTarget = null;
                currentProgress = 0f;
            }
        }
        
        private void SetLanternVisual(bool active)
        {
            if (lanternVisualEffect != null)
                lanternVisualEffect.SetActive(active);
        }
        
        private void PerformScan()
        {
            if (tuning == null)
            {
                Debug.LogWarning("[BlacklightScanner] tuning is null!");
                return;
            }
                
            Vector3 origin = scanOrigin.position;
            Vector3 forward = scanOrigin.forward;
            
            int hitCount = Physics.OverlapSphereNonAlloc(
                origin,
                tuning.scanRange,
                scanResults,
                revealableLayer
            );
            
            IBlacklightRevealable bestCandidate = null;
            float bestScore = float.MaxValue;
            
            for (int i = 0; i < hitCount; i++)
            {
                var collider = scanResults[i];
                if (collider == null)
                    continue;
                    
                var revealable = collider.GetComponent<IBlacklightRevealable>();
                if (revealable == null)
                    continue;
                
                Vector3 toTarget = collider.transform.position - origin;
                float distance = toTarget.magnitude;
                
                if (distance < 0.01f)
                    continue;
                    
                Vector3 directionToTarget = toTarget / distance;
                float angle = Vector3.Angle(forward, directionToTarget);
                
                if (angle > tuning.scanAngle)
                    continue;
                
                float score = distance + (angle * 0.1f);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestCandidate = revealable;
                }
            }
            
            if (bestCandidate != currentTarget)
            {
                if (currentTarget != null)
                    currentTarget.OnRevealInterrupted();
                    
                currentTarget = bestCandidate;
                currentProgress = 0f;
                
                if (currentTarget != null)
                    currentTarget.OnRevealStart();
            }
            
            if (currentTarget != null)
            {
                float progressIncrement = scanInterval / tuning.revealDuration;
                currentProgress = Mathf.Min(1f, currentProgress + progressIncrement);
                currentTarget.OnRevealProgress(currentProgress);
                
                if (currentProgress >= 1f)
                {
                    currentTarget.OnRevealComplete();
                    currentTarget = null;
                    currentProgress = 0f;
                }
            }
        }
        
        private void DecayProgress()
        {
            if (currentTarget == null || tuning == null)
                return;
                
            currentProgress -= tuning.progressDecayRate * Time.deltaTime;
            
            if (currentProgress <= 0f)
            {
                currentTarget.OnRevealInterrupted();
                currentTarget = null;
                currentProgress = 0f;
            }
            else
            {
                currentTarget.OnRevealProgress(currentProgress);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (tuning == null || scanOrigin == null)
                return;
                
            Gizmos.color = isActive ? Color.cyan : Color.gray;
            Gizmos.DrawWireSphere(scanOrigin.position, tuning.scanRange);
            
            Vector3 forward = scanOrigin.forward;
            float halfAngle = tuning.scanAngle * Mathf.Deg2Rad;
            
            Vector3 right = Quaternion.AngleAxis(tuning.scanAngle, scanOrigin.up) * forward;
            Vector3 left = Quaternion.AngleAxis(-tuning.scanAngle, scanOrigin.up) * forward;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(scanOrigin.position, right * tuning.scanRange);
            Gizmos.DrawRay(scanOrigin.position, left * tuning.scanRange);
        }
    }
}
