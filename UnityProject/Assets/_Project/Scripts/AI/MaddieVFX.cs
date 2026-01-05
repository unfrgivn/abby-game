using System.Collections;
using UnityEngine;

namespace WildsOfCloverhollow.AI
{
    /// <summary>
    /// Handles visual effects and animations for Maddie.
    /// Includes idle bobbing and assist attack effects.
    /// </summary>
    public class MaddieVFX : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MaddieTuning tuning;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private Renderer maddieRenderer;
        [SerializeField] private ParticleSystem assistPuffParticle;

        [Header("Audio (Placeholder)")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip assistSoundClip;

        private Vector3 visualRootBasePosition;
        private float bobPhase;
        private MaddieFollower follower;
        private MaddieAssist assist;

        private void Awake()
        {
            follower = GetComponent<MaddieFollower>();
            assist = GetComponent<MaddieAssist>();

            if (visualRoot != null)
            {
                visualRootBasePosition = visualRoot.localPosition;
            }

            if (tuning == null)
            {
                Debug.LogWarning("[MaddieVFX] MaddieTuning reference is missing. Idle bob disabled.");
            }
        }

        private void Update()
        {
            UpdateIdleBob();
        }

        private void UpdateIdleBob()
        {
            if (tuning == null || visualRoot == null) return;

            // Only bob when idle (not moving fast or assisting)
            bool shouldBob = true;
            if (assist != null && assist.IsAssisting)
            {
                shouldBob = false;
            }

            if (shouldBob)
            {
                bobPhase += Time.deltaTime * tuning.IdleBobSpeed;
                float bobOffset = Mathf.Sin(bobPhase * Mathf.PI * 2f) * tuning.IdleBobAmount;
                visualRoot.localPosition = visualRootBasePosition + Vector3.up * bobOffset;
            }
            else
            {
                // Smoothly return to base position
                visualRoot.localPosition = Vector3.Lerp(
                    visualRoot.localPosition,
                    visualRootBasePosition,
                    10f * Time.deltaTime
                );
            }
        }

        /// <summary>
        /// Play the assist attack effect (puff + flash + sound).
        /// Called by MaddieAssist when damage is dealt.
        /// </summary>
        public void PlayAssistEffect()
        {
            // Puff particle
            if (assistPuffParticle != null)
            {
                assistPuffParticle.Play();
            }

            // Color flash
            if (maddieRenderer != null)
            {
                StartCoroutine(AssistFlashCoroutine());
            }

            // Sound cue
            if (audioSource != null && assistSoundClip != null)
            {
                audioSource.PlayOneShot(assistSoundClip);
            }
            else
            {
                // Placeholder: log that sound would play
                Debug.Log("[MaddieVFX] Assist sound cue (placeholder)");
            }
        }

        private IEnumerator AssistFlashCoroutine()
        {
            if (maddieRenderer == null) yield break;

            // Flash to a bright color briefly
            Color originalColor = maddieRenderer.material.color;
            Color flashColor = new Color(1f, 0.9f, 0.5f); // Warm yellow flash

            maddieRenderer.material.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            maddieRenderer.material.color = originalColor;
        }

        /// <summary>
        /// Set up references at runtime if needed.
        /// </summary>
        public void SetVisualRoot(Transform root)
        {
            visualRoot = root;
            if (visualRoot != null)
            {
                visualRootBasePosition = visualRoot.localPosition;
            }
        }

        /// <summary>
        /// Set the renderer reference at runtime if needed.
        /// </summary>
        public void SetRenderer(Renderer renderer)
        {
            maddieRenderer = renderer;
        }
    }
}
