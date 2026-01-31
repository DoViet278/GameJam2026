using UnityEngine;

public class PathMover : MonoBehaviour
{
    public Pathway pathway;
    public float moveSpeed = 2f;
    public bool loop = true;
    public float arriveDistance = 0.05f;
    public float waitTimeAtPoint = 0f;
    public bool startFromClosest = true;

    public Vector2 CurrentVelocity { get; private set; }
    public Vector2 MoveDirection { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsWaiting => isWaiting;

    public bool HasPath => pathway != null && pathway.PointCount > 0;

    private Rigidbody2D rb;
    private int currentIndex = -1;
    private bool isWaiting;
    private float waitTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ResetPath()
    {
        currentIndex = -1;
        isWaiting = false;
        waitTimer = 0f;
    }

    public Vector2 Tick(float deltaTime)
    {
        if (!HasPath || pathway.PointCount < 2)
        {
            SetStopped();
            return Vector2.zero;
        }

        Vector2 currentPos = rb != null ? rb.position : (Vector2)transform.position;

        if (currentIndex < 0)
            currentIndex = startFromClosest ? FindClosestIndex(currentPos) : 0;

        Transform target = pathway.GetPoint(currentIndex);
        if (target == null)
        {
            SetStopped();
            return Vector2.zero;
        }

        Vector2 targetPos = target.position;
        float distance = Vector2.Distance(currentPos, targetPos);

        if (distance <= arriveDistance)
        {
            if (waitTimeAtPoint > 0f)
            {
                if (!isWaiting)
                {
                    isWaiting = true;
                    waitTimer = waitTimeAtPoint;
                    SetStopped();
                    return Vector2.zero;
                }

                waitTimer -= deltaTime;
                if (waitTimer > 0f)
                {
                    SetStopped();
                    return Vector2.zero;
                }

                isWaiting = false;
            }

            if (!AdvanceIndex())
            {
                SetStopped();
                return Vector2.zero;
            }

            target = pathway.GetPoint(currentIndex);
            if (target == null)
            {
                SetStopped();
                return Vector2.zero;
            }

            targetPos = target.position;
        }

        Vector2 direction = (targetPos - currentPos).normalized;
        Vector2 velocity = direction * moveSpeed;

        Move(deltaTime, velocity);
        SetMoving(velocity, direction);

        return direction;
    }

    private void Move(float deltaTime, Vector2 velocity)
    {
        if (rb != null)
        {
            rb.MovePosition(rb.position + velocity * deltaTime);
            return;
        }

        Vector3 pos = transform.position;
        pos.x += velocity.x * deltaTime;
        pos.y += velocity.y * deltaTime;
        transform.position = pos;
    }

    private void SetMoving(Vector2 velocity, Vector2 direction)
    {
        CurrentVelocity = velocity;
        IsMoving = velocity.sqrMagnitude > 0.0001f;
        if (IsMoving)
            MoveDirection = direction;
    }

    private void SetStopped()
    {
        CurrentVelocity = Vector2.zero;
        IsMoving = false;
    }

    private bool AdvanceIndex()
    {
        int nextIndex = currentIndex + 1;
        if (nextIndex >= pathway.PointCount)
        {
            if (!loop)
                return false;

            nextIndex = 0;
        }

        currentIndex = nextIndex;
        return true;
    }

    private int FindClosestIndex(Vector2 currentPos)
    {
        int closestIndex = 0;
        float closestDist = float.MaxValue;

        for (int i = 0; i < pathway.PointCount; i++)
        {
            Transform point = pathway.GetPoint(i);
            if (point == null)
                continue;

            float dist = ((Vector2)point.position - currentPos).sqrMagnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}
