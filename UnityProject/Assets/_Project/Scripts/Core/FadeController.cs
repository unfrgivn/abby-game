using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace WildsOfCloverhollow.Core
{
    /// <summary>
    /// Controls fade in/out transitions using a CanvasGroup.
    /// Used by SceneDirector for smooth scene transitions.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeController : MonoBehaviour
    {
        private CanvasGroup canvasGroup;

        public bool IsFading { get; private set; }

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            SetAlpha(0f);
        }

        public Coroutine FadeOut(float duration)
        {
            return StartCoroutine(FadeCoroutine(0f, 1f, duration));
        }

        public Coroutine FadeIn(float duration)
        {
            return StartCoroutine(FadeCoroutine(1f, 0f, duration));
        }

        public void SetFadeImmediate(bool faded)
        {
            SetAlpha(faded ? 1f : 0f);
        }

        private IEnumerator FadeCoroutine(float from, float to, float duration)
        {
            IsFading = true;

            if (duration <= 0f)
            {
                SetAlpha(to);
                IsFading = false;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                SetAlpha(Mathf.Lerp(from, to, t));
                yield return null;
            }

            SetAlpha(to);
            IsFading = false;
        }

        private void SetAlpha(float alpha)
        {
            canvasGroup.alpha = alpha;
            canvasGroup.blocksRaycasts = alpha > 0.5f;
        }
    }
}
