using UnityEngine;

public class Vision : MonoBehaviour
{
    public float viewRadius = 5f;
    [Range(0f, 360f)]
    public float viewAngle = 120f;
    public LayerMask obstructionMask;

    public bool drawGizmos = true;
    public bool drawWhenSelected = true;
    public Color gizmoColor = new Color(1f, 0.9f, 0.2f, 0.35f);

    public bool drawRuntime = true;
    [Range(3, 64)]
    public int runtimeSegments = 24;
    public Color runtimeColor = new Color(1f, 0.9f, 0.2f, 0.2f);
    public string runtimeSortingLayer = "Default";
    public int runtimeSortingOrder = 0;

    public Transform debugTarget;
    public bool logPlayerDetected = true;

    public bool DidSeePlayerThisFrame { get; private set; }

    private Vector2 lookDirection = Vector2.right;
    private Mesh runtimeMesh;
    private MeshFilter runtimeMeshFilter;
    private MeshRenderer runtimeMeshRenderer;
    private MaterialPropertyBlock runtimeMPB;

    private void Awake()
    {
        if (drawRuntime)
            EnsureRuntimeCone();
    }

    private void OnEnable()
    {
        if (drawRuntime)
            EnsureRuntimeCone();

        UpdateRuntimeCone();
    }

    private void LateUpdate()
    {
        if (drawRuntime)
            UpdateRuntimeCone();

        DidSeePlayerThisFrame = false;
        if (logPlayerDetected)
            CheckForPlayerInView();
    }

    public bool CanSee(Transform target)
    {
        if (target == null)
            return false;

        Vector2 origin = transform.position;
        Vector2 targetPos = target.position;

        if (Vector2.Distance(origin, targetPos) > viewRadius)
            return false;

        if (lookDirection.sqrMagnitude < 0.0001f)
            return false;

        Vector2 toTarget = (targetPos - origin).normalized;
        float halfAngle = viewAngle * 0.5f;
        if (Vector2.Angle(lookDirection, toTarget) > halfAngle)
            return false;

        if (obstructionMask.value != 0)
        {
            RaycastHit2D hit = Physics2D.Linecast(origin, targetPos, obstructionMask);
            if (hit.collider != null)
                return false;
        }

        return true;
    }

    private void CheckForPlayerInView()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, viewRadius);
        if (hits == null || hits.Length == 0)
            return;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            if (!hits[i].CompareTag("Player"))
                continue;

            Transform target = hits[i].transform;
            if (CanSee(target))
            {
                DidSeePlayerThisFrame = true;
                Debug.Log("End Game");
                return;
            }
        }
    }

    public void SetLookDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.0001f)
            lookDirection = dir.normalized;
    }

    private void EnsureRuntimeCone()
    {
        if (runtimeMeshFilter == null || runtimeMeshRenderer == null)
        {
            Transform existing = transform.Find("VisionCone");
            if (existing != null)
            {
                runtimeMeshFilter = existing.GetComponent<MeshFilter>();
                runtimeMeshRenderer = existing.GetComponent<MeshRenderer>();
            }
        }

        if (runtimeMeshFilter == null || runtimeMeshRenderer == null)
        {
            GameObject cone = new GameObject("VisionCone");
            cone.transform.SetParent(transform, false);
            cone.transform.localPosition = Vector3.zero;
            cone.transform.localRotation = Quaternion.identity;
            cone.transform.localScale = Vector3.one;

            runtimeMeshFilter = cone.AddComponent<MeshFilter>();
            runtimeMeshRenderer = cone.AddComponent<MeshRenderer>();
        }

        if (runtimeMesh == null)
        {
            runtimeMesh = new Mesh();
            runtimeMesh.name = "VisionConeMesh";
            runtimeMeshFilter.mesh = runtimeMesh;
        }

        if (runtimeMeshRenderer.sharedMaterial == null)
        {
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
                shader = Shader.Find("Unlit/Color");

            runtimeMeshRenderer.sharedMaterial = new Material(shader);
        }

        runtimeMeshRenderer.sortingLayerName = runtimeSortingLayer;
        runtimeMeshRenderer.sortingOrder = runtimeSortingOrder;

        if (runtimeMPB == null)
            runtimeMPB = new MaterialPropertyBlock();
    }

    private void UpdateRuntimeCone()
    {
        if (runtimeMeshFilter == null || runtimeMeshRenderer == null)
            return;

        runtimeMeshRenderer.enabled = drawRuntime;
        if (!drawRuntime)
            return;

        if (runtimeMesh == null)
        {
            runtimeMesh = new Mesh();
            runtimeMesh.name = "VisionConeMesh";
            runtimeMeshFilter.mesh = runtimeMesh;
        }

        int segments = Mathf.Clamp(runtimeSegments, 3, 64);
        Vector2 forward = lookDirection.sqrMagnitude > 0.0001f ? lookDirection.normalized : Vector2.right;
        float halfAngle = viewAngle * 0.5f;
        float step = viewAngle / segments;
        float scale = Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y));
        float radiusLocal = scale > 0.0001f ? viewRadius / scale : viewRadius;

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float angle = -halfAngle + step * i;
            Vector2 worldDir = Rotate(forward, angle);
            Vector3 localDir = transform.InverseTransformDirection(worldDir);
            Vector3 point = localDir.normalized * radiusLocal;
            vertices[i + 1] = point;
        }

        for (int i = 0; i < segments; i++)
        {
            int triIndex = i * 3;
            triangles[triIndex] = 0;
            triangles[triIndex + 1] = i + 1;
            triangles[triIndex + 2] = i + 2;
        }

        runtimeMesh.Clear();
        runtimeMesh.vertices = vertices;
        runtimeMesh.triangles = triangles;
        runtimeMesh.RecalculateBounds();

        runtimeMPB.SetColor("_Color", runtimeColor);
        runtimeMeshRenderer.SetPropertyBlock(runtimeMPB);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos || !drawWhenSelected)
            return;

        DrawViewCone();
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || drawWhenSelected)
            return;

        DrawViewCone();
    }

    private void DrawViewCone()
    {
        Vector2 origin = transform.position;
        Vector2 forward = lookDirection.sqrMagnitude > 0.0001f ? lookDirection.normalized : Vector2.right;

        float halfAngle = viewAngle * 0.5f;
        int segments = 24;
        float step = viewAngle / segments;

        Gizmos.color = gizmoColor;

        Vector2 leftDir = Rotate(forward, -halfAngle);
        Vector2 rightDir = Rotate(forward, halfAngle);

        Gizmos.DrawLine(origin, origin + leftDir * viewRadius);
        Gizmos.DrawLine(origin, origin + rightDir * viewRadius);

        Vector2 prevDir = leftDir;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -halfAngle + step * i;
            Vector2 nextDir = Rotate(forward, angle);
            Gizmos.DrawLine(origin + prevDir * viewRadius, origin + nextDir * viewRadius);
            prevDir = nextDir;
        }

        if (debugTarget != null)
        {
            bool canSee = CanSee(debugTarget);
            Gizmos.color = canSee ? Color.green : Color.red;
            Gizmos.DrawLine(origin, debugTarget.position);
        }
    }

    private static Vector2 Rotate(Vector2 dir, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(dir.x * cos - dir.y * sin, dir.x * sin + dir.y * cos);
    }
}
