using UnityEngine;
using UnityEngine.EventSystems;

namespace WildsOfCloverhollow.UI
{
    /// <summary>
    /// Touch joystick exposing normalized Vector2 for InputRouter to read.
    /// Implements pointer interfaces directly instead of OnScreenStick (which lacks virtual methods).
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Joystick Settings")]
        [SerializeField] private float movementRange = 50f;
        [SerializeField] private bool dynamicOrigin = true;

        [Header("References")]
        [SerializeField] private RectTransform knob;

        private RectTransform rectTransform;
        private RectTransform parentRect;
        private Vector2 startPosition;
        private Vector2 inputOrigin;
        private Canvas parentCanvas;
        private UnityEngine.Camera canvasCamera;

        /// <summary>Joystick value normalized to -1..1 per axis.</summary>
        public Vector2 Value { get; private set; }

        /// <summary>True while joystick is being touched.</summary>
        public bool IsActive { get; private set; }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            parentRect = transform.parent as RectTransform;
            startPosition = rectTransform.anchoredPosition;
            parentCanvas = GetComponentInParent<Canvas>();

            if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                canvasCamera = parentCanvas.worldCamera;
            }

            if (knob == null)
            {
                var knobTransform = transform.Find("Knob");
                if (knobTransform != null)
                {
                    knob = knobTransform as RectTransform;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsActive = true;

            if (dynamicOrigin && parentRect != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    eventData.position,
                    canvasCamera,
                    out var localPoint);

                rectTransform.anchoredPosition = localPoint;
                inputOrigin = localPoint;
            }
            else
            {
                inputOrigin = rectTransform.anchoredPosition;
            }

            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!IsActive) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                canvasCamera,
                out var localPoint);

            Vector2 delta = localPoint - inputOrigin;

            if (delta.magnitude > movementRange)
            {
                delta = delta.normalized * movementRange;
            }

            Value = delta / movementRange;

            if (knob != null)
            {
                knob.anchoredPosition = delta;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsActive = false;
            Value = Vector2.zero;

            if (knob != null)
            {
                knob.anchoredPosition = Vector2.zero;
            }

            if (dynamicOrigin)
            {
                rectTransform.anchoredPosition = startPosition;
            }
        }

        private void OnDisable()
        {
            IsActive = false;
            Value = Vector2.zero;

            if (knob != null)
            {
                knob.anchoredPosition = Vector2.zero;
            }
        }
    }
}
