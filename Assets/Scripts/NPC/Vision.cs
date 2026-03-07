using System.Collections;
using UnityEngine;

public class Vision : MonoBehaviour
{
    public float viewRadius = 5f;
    [Range(0f, 360f)]
    public float viewAngle = 120f;
    [Tooltip("Tag used to identify targets. Leave empty to allow any tag.")]
    public string targetTag = "Player";
    [Tooltip("Only consider colliders on these layers. Leave empty to include all layers.")]
    public LayerMask targetMask;
    [Tooltip("Ignore trigger colliders when checking for targets in view.")]
    public bool ignoreTriggerColliders = true;
    [Tooltip("Require a PlayerController on the target or its parents.")]
    public bool requirePlayerComponent = true;
    [Tooltip("Use NPC tag rules (Boss sees all, Security ignores Security, Staff ignores Staff).")]
    public bool useNpcTagRules = true;
    public string bossTag = "Boss";
    public string securityTag = "Security";
    public string staffTag = "Staff";
    public LayerMask obstructionMask;
    [Tooltip("Tag used for walls that block vision. Leave empty to ignore tags.")]
    public string wallTag = "Wall";
    [Tooltip("Optional transform used as the origin for vision checks and gizmos. If null, uses this transform.")]
    public Transform viewOrigin;
    [Tooltip("Optional transform that defines the look direction (its right vector). If null, uses viewOrigin then this transform.")]
    public Transform lookDirectionSource;
    [Tooltip("Sync look direction to transform rotation when it changes and no manual look direction was set that frame.")]
    public bool syncLookDirectionToRotation = true;
    [Tooltip("Minimum rotation change (degrees) needed to sync look direction.")]
    public float rotationSyncThreshold = 0.1f;

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

    private enum NpcRole
    {
        None,
        Boss,
        Security,
        Staff
    }

    private enum PlayerRole
    {
        Unknown,
        Player,
        Staff,
        Security
    }

    private Vector2 lookDirection = Vector2.right;
    private Mesh runtimeMesh;
    private MeshFilter runtimeMeshFilter;
    private MeshRenderer runtimeMeshRenderer;
    private MaterialPropertyBlock runtimeMPB;
    private RaycastHit2D[] obstructionHits = new RaycastHit2D[16];
    private int lastLookDirectionFrame = -1;
    private float lastRotationZ;

    private void Awake()
    {
        TryResolveEndGameUI();
        if (drawRuntime)
            EnsureRuntimeCone();
        lastRotationZ = transform.eulerAngles.z;
        if (!TryUpdateLookDirectionFromSource() && syncLookDirectionToRotation && lastLookDirectionFrame < 0)
        {
            Vector2 dir = transform.right;
            if (dir.sqrMagnitude > 0.0001f)
                lookDirection = dir.normalized;
        }
    }

    private void OnEnable()
    {
        TryResolveEndGameUI();
        EndGameTriggered = false;
        if (drawRuntime)
            EnsureRuntimeCone();

        lastRotationZ = transform.eulerAngles.z;
        if (!TryUpdateLookDirectionFromSource() && syncLookDirectionToRotation && lastLookDirectionFrame < 0)
        {
            Vector2 dir = transform.right;
            if (dir.sqrMagnitude > 0.0001f)
                lookDirection = dir.normalized;
        }
        UpdateRuntimeCone();
    }

    private void LateUpdate()
    {
        SyncLookDirectionToRotation();
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

        return CanSeeInternal(target.position, target);
    }

    public bool CanDetect(Transform target)
    {
        if (!TryResolveTarget(target, out Transform tagTarget))
            return false;

        if (!IsTargetAllowed(tagTarget))
            return false;

        return CanSeeInternal(tagTarget.position, tagTarget);
    }

    private void CheckForPlayerInView()
    {
        Vector2 origin = GetOrigin();
        int mask = targetMask.value != 0 ? targetMask.value : Physics2D.AllLayers;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, viewRadius, mask);
        if (hits == null || hits.Length == 0)
            return;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            if (ignoreTriggerColliders && hits[i].isTrigger)
                continue;

            Transform target = hits[i].attachedRigidbody != null ? hits[i].attachedRigidbody.transform : hits[i].transform;
            if (CanDetect(target))
            {
                DidSeePlayerThisFrame = true;
                TriggerEndGame();
                return;
            }
        }
    }

    public void SetLookDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.0001f)
        {
            lookDirection = dir.normalized;
            lastLookDirectionFrame = Time.frameCount;
        }
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
        Vector2 origin = GetOrigin();
        Vector3 localOrigin = viewOrigin != null ? transform.InverseTransformPoint(origin) : Vector3.zero;

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = localOrigin;

        for (int i = 0; i <= segments; i++)
        {
            float angle = -halfAngle + step * i;
            Vector2 worldDir = Rotate(forward, angle);
            float distance = viewRadius;
            if (TryGetWallHit(origin, worldDir, viewRadius, out float hitDistance))
                distance = hitDistance;

            Vector3 localDir = transform.InverseTransformDirection(worldDir);
            float localDistance = scale > 0.0001f ? distance / scale : distance;
            Vector3 point = localOrigin + localDir.normalized * localDistance;
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
        Vector2 origin = GetOrigin();
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

    private bool CanSeeInternal(Vector2 targetPos, Transform ignoreTransform)
    {
        Vector2 origin = GetOrigin();

        if (Vector2.Distance(origin, targetPos) > viewRadius)
            return false;

        if (lookDirection.sqrMagnitude < 0.0001f)
            return false;

        Vector2 toTarget = (targetPos - origin).normalized;
        float halfAngle = viewAngle * 0.5f;
        if (Vector2.Angle(lookDirection, toTarget) > halfAngle)
            return false;

        float distance = Vector2.Distance(origin, targetPos);
        if (TryGetWallHit(origin, toTarget, distance, out _, ignoreTransform))
            return false;

        return true;
    }

    private bool TryResolveTarget(Transform target, out Transform tagTarget)
    {
        tagTarget = target;
        if (target == null)
            return false;

        if (!requirePlayerComponent)
            return true;

        PlayerController player = target.GetComponentInParent<PlayerController>();
        if (player == null)
            return false;

        tagTarget = player.transform;
        return true;
    }

    private bool IsTargetAllowed(Transform target)
    {
        if (target == null)
            return false;

        NpcRole npcRole = GetNpcRole();
        if (npcRole == NpcRole.Boss)
            return true;

        if (useNpcTagRules)
        {
            if (npcRole != NpcRole.None)
            {
                PlayerRole playerRole = GetPlayerRole(target);
                if (playerRole != PlayerRole.Unknown)
                {
                    switch (npcRole)
                    {
                        case NpcRole.Boss:
                            return playerRole == PlayerRole.Player ||
                                playerRole == PlayerRole.Staff ||
                                playerRole == PlayerRole.Security;
                        case NpcRole.Security:
                            return playerRole == PlayerRole.Player || playerRole == PlayerRole.Staff;
                        case NpcRole.Staff:
                            return playerRole == PlayerRole.Player;
                    }
                }
            }

            if (!string.IsNullOrEmpty(bossTag) && CompareTag(bossTag))
                return true;

            if (!string.IsNullOrEmpty(securityTag) && CompareTag(securityTag))
                return !target.CompareTag(securityTag);

            if (!string.IsNullOrEmpty(staffTag) && CompareTag(staffTag))
                return !target.CompareTag(staffTag);
        }

        if (string.IsNullOrEmpty(targetTag))
            return true;

        return target.CompareTag(targetTag);
    }

    private NpcRole GetNpcRole()
    {
        if (TryGetComponent<BossNPC>(out _))
            return NpcRole.Boss;
        if (TryGetComponent<SecurityNPC>(out _))
            return NpcRole.Security;
        if (TryGetComponent<StaffNPC>(out _))
            return NpcRole.Staff;

        if (!string.IsNullOrEmpty(bossTag) && CompareTag(bossTag))
            return NpcRole.Boss;
        if (!string.IsNullOrEmpty(securityTag) && CompareTag(securityTag))
            return NpcRole.Security;
        if (!string.IsNullOrEmpty(staffTag) && CompareTag(staffTag))
            return NpcRole.Staff;

        return NpcRole.None;
    }

    private PlayerRole GetPlayerRole(Transform target)
    {
        PlayerController player = target.GetComponent<PlayerController>();
        if (player == null)
            return PlayerRole.Unknown;

        if (GameController.instance != null)
        {
            switch (GameController.instance.index)
            {
                case 0:
                    return PlayerRole.Player;
                case 1:
                    return PlayerRole.Staff;
                case 2:
                    return PlayerRole.Security;
            }
        }

        if (!string.IsNullOrEmpty(staffTag) && player.CompareTag(staffTag))
            return PlayerRole.Staff;
        if (!string.IsNullOrEmpty(securityTag) && player.CompareTag(securityTag))
            return PlayerRole.Security;
        if (string.IsNullOrEmpty(targetTag) || player.CompareTag(targetTag))
            return PlayerRole.Player;

        return PlayerRole.Unknown;
    }

    private void SyncLookDirectionToRotation()
    {
        if (TryUpdateLookDirectionFromSource())
            return;

        if (!syncLookDirectionToRotation)
            return;

        float currentZ = transform.eulerAngles.z;
        bool rotationChanged = Mathf.Abs(Mathf.DeltaAngle(lastRotationZ, currentZ)) > rotationSyncThreshold;
        if (rotationChanged && lastLookDirectionFrame != Time.frameCount)
        {
            Vector2 dir = transform.right;
            if (dir.sqrMagnitude > 0.0001f)
                lookDirection = dir.normalized;
        }

        lastRotationZ = currentZ;
    }

    private bool TryUpdateLookDirectionFromSource()
    {
        if (lastLookDirectionFrame == Time.frameCount)
            return false;

        Transform source = lookDirectionSource != null ? lookDirectionSource : viewOrigin;
        if (source == null)
            return false;

        Vector2 dir = source.right;
        if (dir.sqrMagnitude > 0.0001f)
        {
            lookDirection = dir.normalized;
            return true;
        }

        return false;
    }

    private Vector2 GetOrigin()
    {
        return viewOrigin != null ? (Vector2)viewOrigin.position : (Vector2)transform.position;
    }

    private void TriggerEndGame()
    {
        if (!triggerEndGameOnDetect || EndGameTriggered)
            return;

        EndGameTriggered = true;
        TryResolveEndGameUI();
        StartCoroutine(delayGameOver());
    }

    private void TryResolveEndGameUI()
    {
        if (!autoFindEndGameUI || endGameUI != null)
            return;

        endGameUI = FindObjectOfType<EndGameUI>(true);
    }

    private IEnumerator delayGameOver()
    {
        yield return new WaitForSeconds(0.5f);
        GameController.instance.isGameOver = true;
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
        bool checkWallTag = !string.IsNullOrEmpty(wallTag);
        if (!useMask && !checkWallTag)
            return false;

        int mask = (useMask && !checkWallTag) ? obstructionMask.value : Physics2D.AllLayers;
        int hitCount = Physics2D.RaycastNonAlloc(origin, direction, obstructionHits, maxDistance, mask);
        if (hitCount <= 0)
            return false;

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

            bool blocksByLayer = useMask && ((1 << col.gameObject.layer) & obstructionMask.value) != 0;
            bool blocksByTag = checkWallTag && col.CompareTag(wallTag);
            if (!blocksByLayer && !blocksByTag)
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
