using UnityEngine;
using UnityEngine.InputSystem;

namespace WildsOfCloverhollow.UI
{
    public class ControlsPanel : MonoBehaviour
    {
        private bool isVisible;

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                isVisible = !isVisible;
            }
        }

        private void OnGUI()
        {
            if (!isVisible) return;

            float boxWidth = 400f;
            float boxHeight = 320f;
            float x = (Screen.width - boxWidth) / 2f;
            float y = (Screen.height - boxHeight) / 2f;

            GUI.Box(new Rect(x, y, boxWidth, boxHeight), "");

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            GUIStyle controlStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleLeft
            };

            GUIStyle hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter
            };

            float padding = 20f;
            float lineHeight = 28f;
            float currentY = y + padding;

            GUI.Label(new Rect(x, currentY, boxWidth, 30f), "Controls", titleStyle);
            currentY += 40f;

            string[] controls = new string[]
            {
                "WASD / Left Stick    Move",
                "Two-finger Scroll    Rotate Camera",
                "Right Stick          Rotate Camera",
                "F / North Button     Attack",
                "R / East Button      Dodge",
                "E / South Button     Interact",
                "J                    Journal",
                "L                    Lantern",
                "F1                   Debug Overlay",
                "Esc                  This Menu"
            };

            foreach (string control in controls)
            {
                GUI.Label(new Rect(x + padding, currentY, boxWidth - padding * 2, lineHeight), control, controlStyle);
                currentY += lineHeight;
            }

            currentY += 10f;
            GUI.Label(new Rect(x, currentY, boxWidth, 20f), "Press Esc to close", hintStyle);
        }
    }
}
