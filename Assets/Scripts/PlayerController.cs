using System;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public event Action onFlipped;

    [SerializeField] private RuntimeAnimatorController player;
    [SerializeField] private RuntimeAnimatorController staff;
    [SerializeField] private RuntimeAnimatorController security;

    public PlayerInputSet input { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerMoveUDState moveUDState { get; private set; }
    public PlayerMoveUpState moveUpState { get; private set; }
    public PlayerSearchState searchState { get; private set; }
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Vector2 moveInput { get; private set; }

    public PlayerStateMachine stateMachine;

    public float moveSpeed = 5f;

    private bool facingLeft = true;
    public int facingDirLeft { get; private set; } = -1;

    [Header("Collision detection")]
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform ground2Check;
    [SerializeField] private Transform primaryWallCheck;

    [Header("CheckShelf")]
    [SerializeField] protected LayerMask whatIsShelf;
    [SerializeField] protected LayerMask whatIsClothesStaff;
    [SerializeField] protected LayerMask whatIsClothesSecurity;

    public bool groundDetected { get; private set; }
    public bool ground2Detected { get; private set; }
    public bool wallDetected { get; private set; }

    private TimeToMask timeToMask;
    private TimeToSearch timeToSearch;

    public bool isDisguiseStaff { get; private set; }
    public bool isDisguiseSecurity { get; private set; }
    public bool researchShelf { get; private set; }

    public bool isSearching = false;


    void Awake()
    {
        stateMachine = new PlayerStateMachine();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        input = new PlayerInputSet();
        idleState = new PlayerIdleState(this, stateMachine, "idle");
        moveState = new PlayerMoveState(this, stateMachine, "moveLR");
        moveUDState = new PlayerMoveUDState(this, stateMachine, "moveUD");
        moveUpState = new PlayerMoveUpState(this, stateMachine, "moveUD");
        searchState = new PlayerSearchState(this, stateMachine, "search");
        timeToMask = GetComponent<TimeToMask>();
        timeToSearch = GetComponent<TimeToSearch>();
    }

    private void Start()
    {
        stateMachine.Init(idleState);
    }
    void OnEnable()
    {
        input.Player.Enable();
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    private void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isDisguiseStaff)
            {
                GameController.instance.index = 1;
                timeToMask.StartToMask();
            }
            if (isDisguiseSecurity)
            {
                GameController.instance.index = 2;
                timeToMask.StartToMask();
            }
            if (researchShelf)
            {
                isSearching = true;
                stateMachine.ChangeState(searchState);
                timeToSearch.StartSearch();
            }
        }

        if(GameController.instance.index == 0) anim.runtimeAnimatorController = player;
        else if(GameController.instance.index == 1) anim.runtimeAnimatorController = staff;
        else if(GameController.instance.index == 2) anim.runtimeAnimatorController = security;
    }

    void OnDisable()
    {
        input.Player.Disable();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.velocity = new Vector2(xVelocity, yVelocity);
        HandleFlipLeft(xVelocity);
    }

    public void HandleFlipLeft(float xVelocity)
    {
        if (xVelocity > 0 && facingLeft == true)
        {
            FlipLeftRight();
        }
        else if (xVelocity < 0 && facingLeft == false)
        {
            FlipLeftRight();
        }
    }

    public void FlipLeftRight()
    {
        transform.Rotate(0, 180, 0);
        facingLeft = !facingLeft;
        facingDirLeft = facingDirLeft * -1;
        onFlipped?.Invoke();
    }

    private void HandleCollisionDetection()
    {
        researchShelf = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsShelf) ||
           Physics2D.Raycast(groundCheck.position, Vector2.up, groundCheckDistance, whatIsShelf) ||
           Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDirLeft, wallCheckDistance, whatIsShelf);

        isDisguiseStaff =
           Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDirLeft, wallCheckDistance, whatIsClothesStaff);

        isDisguiseSecurity =
            Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDirLeft, wallCheckDistance, whatIsClothesSecurity);

    }
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(ground2Check.position, groundCheck.position + new Vector3(0, groundCheckDistance));
        Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDirLeft, 0));
    }
}