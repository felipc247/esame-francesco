using UnityEngine;

[ExecuteInEditMode]
public class PathGizmo : MonoBehaviour
{
    public Color lineColor = Color.yellow;

    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;

        Transform[] children = GetComponentsInChildren<Transform>();

        for (int i = 1; i < children.Length - 1; i++)
        {
            Transform current = children[i];
            Transform next = children[i + 1];

            if (current != null && next != null)
            {
                Gizmos.DrawLine(current.position, next.position);
            }
        }

        for (int i = 1; i < children.Length; i++)
        {
            Gizmos.DrawSphere(children[i].position, 0.1f);
        }
    }
}
