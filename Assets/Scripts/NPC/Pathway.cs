using UnityEngine;

public class Pathway : MonoBehaviour
{
    public Transform[] points;

    public int PointCount => points == null ? 0 : points.Length;

    public Transform GetPoint(int index)
    {
        if (points == null || index < 0 || index >= points.Length)
            return null;

        return points[index];
    }

    private void OnDrawGizmosSelected()
    {
        if (points == null || points.Length == 0)
            return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < points.Length; i++)
        {
            Transform current = points[i];
            if (current == null)
                continue;

            Gizmos.DrawSphere(current.position, 0.05f);

            int nextIndex = i + 1;
            if (nextIndex >= points.Length)
                continue;

            Transform next = points[nextIndex];
            if (next == null)
                continue;

            Gizmos.DrawLine(current.position, next.position);
        }
    }
}
