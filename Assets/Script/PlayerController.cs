using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private PlayerInputActions input;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    private void OnMove()
    {
        moveInput = input.Player.Move.ReadValue<Vector2>();
        Debug.Log(moveInput);
    }

    void FixedUpdate()
    {
        Vector2 move = moveInput;
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }
    void Update()
    {
        OnMove();
    }
}