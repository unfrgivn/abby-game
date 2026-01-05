using UnityEngine;
using WildsOfCloverhollow.Core;

namespace WildsOfCloverhollow.World
{
    [RequireComponent(typeof(Collider))]
    public class SceneTransitionTrigger : MonoBehaviour
    {
        [SerializeField] private string targetScene;
        [SerializeField] private string targetAnchorId;
        [SerializeField] private string interactionPrompt = "Enter";

        private bool _isTransitioning;

        public string TargetScene => targetScene;
        public string TargetAnchorId => targetAnchorId;
        public string InteractionPrompt => interactionPrompt;

        private void OnTriggerEnter(Collider other)
        {
            if (_isTransitioning) return;
            if (!other.CompareTag("Player")) return;

            TriggerTransition();
        }

        public void TriggerTransition()
        {
            if (_isTransitioning) return;
            if (string.IsNullOrEmpty(targetScene)) return;

            var sceneDirector = SceneDirector.Instance;
            if (sceneDirector == null || sceneDirector.IsTransitioning) return;

            _isTransitioning = true;
            sceneDirector.LoadScene(targetScene, targetAnchorId);
        }

        private void OnDisable()
        {
            _isTransitioning = false;
        }

#if UNITY_EDITOR
        public void SetTargetScene(string scene) => targetScene = scene;
        public void SetTargetAnchorId(string anchorId) => targetAnchorId = anchorId;
        public void SetInteractionPrompt(string prompt) => interactionPrompt = prompt;
#endif
    }
}
