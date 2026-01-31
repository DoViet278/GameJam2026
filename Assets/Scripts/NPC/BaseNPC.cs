using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class BaseNPC : MonoBehaviour
{
    [Header("Components")]
    public Vision vision;
    public PathMover pathMover;

    [Header("References")]
    public Transform player;

    protected Animator anim;
    protected NPCStateMachine fsm;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        fsm = new NPCStateMachine();   // NOT GetComponent
        if (vision == null)
            vision = GetComponent<Vision>();
        if (pathMover == null)
            pathMover = GetComponent<PathMover>();
    }

    protected virtual void Update()
    {
        fsm.Update();
    }

    // ===== Animator shared for all NPCs =====
    public void UpdateAnimator(Vector2 dir)
    {
        bool moving = dir.magnitude > 0.1f;

        anim.SetBool("isMoving", moving);
        anim.SetFloat("moveX", dir.x);
        anim.SetFloat("moveY", dir.y);

        if (moving && vision != null)
            vision.SetLookDirection(dir);
    }

    public void SetIdle()
    {
        anim.SetBool("isMoving", false);
    }

    // ===== Vision used by states =====
    public bool CanSeePlayer()
    {
        return vision != null && vision.CanSee(player);
    }

    // ===== Alert must be overridden =====
    public abstract void Alert();

    // ===== Common state change =====
    public void ChangeState(NPCState newState)
    {
        fsm.ChangeState(newState);
    }
}
