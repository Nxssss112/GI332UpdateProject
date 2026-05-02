using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class SubmarineController : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float strafeSpeed = 10f;
    [SerializeField] private float verticalSpeed = 12f;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 0.09f;
    [SerializeField] private float smoothRotation = 15f;

    private Rigidbody rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float upDownInput;
    private float rotationX;
    private float rotationY;

    private bool isStunned = false;
    private float stunEndTime;
    private float hitCooldownEndTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerInputActions();
        rb.useGravity = false;
        rb.linearDamping = 1.5f;
        rb.angularDamping = 2.5f;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        inputActions.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;
        inputActions.Player.UpDown.performed += OnUpDown;
        inputActions.Player.UpDown.canceled += OnUpDown;

        LockCursor();
    }

    public override void OnNetworkDespawn() => inputActions.Disable();

    private void OnMove(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    private void OnLook(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();
    private void OnUpDown(InputAction.CallbackContext ctx) => upDownInput = ctx.ReadValue<float>();

    private void Update()
    {
        if (!IsOwner) return;

        if (isStunned)
        {
            if (Time.time >= stunEndTime) isStunned = false;
            else return;
        }

        rotationY += lookInput.x * mouseSensitivity;
        rotationX -= lookInput.y * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -70f, 70f);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotationX, rotationY, 0f), Time.deltaTime * smoothRotation);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        if (isStunned)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        Vector3 moveDir = (transform.forward * moveInput.y * moveSpeed) +
                          (transform.right * moveInput.x * strafeSpeed) +
                          (transform.up * upDownInput * verticalSpeed);

        rb.AddForce(moveDir, ForceMode.Acceleration);
    }

    // --- Server Side Logic ---
    public void ApplyHit()
    {
        if (!IsServer) return;
        if (Time.time < hitCooldownEndTime) return;

        hitCooldownEndTime = Time.time + 5f;
        ApplyHitClientRpc();
    }

    // --- Client Side Callbacks ---
    [ClientRpc]
    private void ApplyHitClientRpc()
    {
        if (!IsOwner) return;

        //Debug.Log("I am Stunned!");
        isStunned = true;
        stunEndTime = Time.time + 3f;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    [ClientRpc]
    public void NotifyHitSuccessClientRpc(ClientRpcParams rpcParams = default)
    {
        // แจ้งเตือนคนยิง (ใช้ rpcParams เพื่อระบุ Client ตัวเดียว)
        //Debug.Log("<color=cyan>Target Hit!</color> คุณยิงโดนผู้เล่นคนอื่นแล้ว");
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}