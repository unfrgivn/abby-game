using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace WildsOfCloverhollow.UI
{
    public class VirtualJoystick : OnScreenStick
    {
        [Header("Joystick Settings")]
        [SerializeField] private float movementRange = 50f;
        [SerializeField] private bool dynamicOrigin = true;

        private RectTransform rectTransform;
        private RectTransform parentRect;
        private Vector2 startPosition;
        private Vector2 pointerDownPosition;
        private Canvas parentCanvas;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
            parentRect = transform.parent as RectTransform;
            startPosition = rectTransform.anchoredPosition;
            parentCanvas = GetComponentInParent<Canvas>();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (dynamicOrigin && parentRect != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    eventData.position,
                    eventData.pressEventCamera,
                    out pointerDownPosition);

                rectTransform.anchoredPosition = pointerDownPosition;
            }

            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (dynamicOrigin)
            {
                rectTransform.anchoredPosition = startPosition;
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
        }
    }
}
