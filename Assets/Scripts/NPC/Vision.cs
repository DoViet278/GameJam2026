using UnityEngine;

public class Vision : MonoBehaviour
{
    public float viewRadius = 5f;
    [Range(0f, 360f)]
    public float viewAngle = 120f;
    public LayerMask obstructionMask;
    [Tooltip("Tag used for walls that block vision when obstructionMask is empty. Leave empty to ignore tags.")]
    public string wallTag = "Wall";

    [Header("End Game")]
    [SerializeField] private bool triggerEndGameOnDetect = true;
    [SerializeField] private bool autoFindEndGameUI = true;
    [SerializeField] private EndGameUI endGameUI;

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
    public bool EndGameTriggered { get; private set; }

    private Vector2 lookDirection = Vector2.right;
    private Mesh runtimeMesh;
    private MeshFilter runtimeMeshFilter;
    private MeshRenderer runtimeMeshRenderer;
    private MaterialPropertyBlock runtimeMPB;
    private RaycastHit2D[] obstructionHits = new RaycastHit2D[16];

    private void Awake()
    {
        TryResolveEndGameUI();
        if (drawRuntime)
            EnsureRuntimeCone();
    }

    private void OnEnable()
    {
        TryResolveEndGameUI();
        EndGameTriggered = false;
        if (drawRuntime)
            EnsureRuntimeCone();

        UpdateRuntimeCone();
    }

    private void LateUpdate()
    {
        if (drawRuntime)
            UpdateRuntimeCone();

        DidSeePlayerThisFrame = false;
        if (logPlayerDetected && !EndGameTriggered)
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

        float distance = Vector2.Distance(origin, targetPos);
        if (TryGetWallHit(origin, toTarget, distance, out _, target))
            return false;

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
                TriggerEndGame();
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
            float distance = viewRadius;
            if (TryGetWallHit(transform.position, worldDir, viewRadius, out float hitDistance))
                distance = hitDistance;

            Vector3 localDir = transform.InverseTransformDirection(worldDir);
            float localDistance = scale > 0.0001f ? distance / scale : distance;
            Vector3 point = localDir.normalized * localDistance;
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
        float leftDistance = viewRadius;
        if (TryGetWallHit(origin, leftDir, viewRadius, out float leftHit))
            leftDistance = leftHit;
        Vector2 leftPoint = origin + leftDir * leftDistance;

        Vector2 rightDir = Rotate(forward, halfAngle);
        float rightDistance = viewRadius;
        if (TryGetWallHit(origin, rightDir, viewRadius, out float rightHit))
            rightDistance = rightHit;
        Vector2 rightPoint = origin + rightDir * rightDistance;

        Gizmos.DrawLine(origin, leftPoint);
        Gizmos.DrawLine(origin, rightPoint);

        Vector2 prevPoint = leftPoint;
        for (int i = 1; i <= segments; i++)
        {
            float angle = -halfAngle + step * i;
            Vector2 nextDir = Rotate(forward, angle);
            float nextDistance = viewRadius;
            if (TryGetWallHit(origin, nextDir, viewRadius, out float nextHit))
                nextDistance = nextHit;

            Vector2 nextPoint = origin + nextDir * nextDistance;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
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

    private void TriggerEndGame()
    {
        if (!triggerEndGameOnDetect || EndGameTriggered)
            return;

        EndGameTriggered = true;
        TryResolveEndGameUI();
        GameController.instance.isGameOver = true;
    }

    private void TryResolveEndGameUI()
    {
        if (!autoFindEndGameUI || endGameUI != null)
            return;

        endGameUI = FindObjectOfType<EndGameUI>(true);
    }

    private bool TryGetWallHit(
        Vector2 origin,
        Vector2 direction,
        float maxDistance,
        out float hitDistance,
        Transform ignoreTransform = null)
    {
        hitDistance = maxDistance;

        bool useMask = obstructionMask.value != 0;
        int mask = useMask ? obstructionMask.value : Physics2D.AllLayers;
        int hitCount = Physics2D.RaycastNonAlloc(origin, direction, obstructionHits, maxDistance, mask);
        if (hitCount <= 0)
            return false;

        bool requireTag = !string.IsNullOrEmpty(wallTag) && !useMask;
        float closest = maxDistance;
        bool found = false;

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit2D hit = obstructionHits[i];
            Collider2D col = hit.collider;
            if (col == null)
                continue;

            if (ignoreTransform != null)
            {
                Transform colTransform = col.transform;
                if (colTransform == ignoreTransform || colTransform.IsChildOf(ignoreTransform))
                    continue;
            }

            if (requireTag && !col.CompareTag(wallTag))
                continue;

            if (hit.distance < closest)
            {
                closest = hit.distance;
                found = true;
            }
        }

        if (found)
            hitDistance = closest;

        return found;
    }
}
