using UnityEngine;

namespace WildsOfCloverhollow.Tools
{
    public class NoteGizmo : MonoBehaviour
    {
        [SerializeField] private Color gizmoColor = new Color(0.5f, 0f, 1f, 0.8f);
        [SerializeField] private float gizmoRadius = 0.5f;

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, gizmoRadius);
            Gizmos.DrawIcon(transform.position + Vector3.up * 0.5f, "d_UnityEditor.ConsoleWindow@2x", true);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, gizmoRadius * 0.3f);
        }
    }
}
