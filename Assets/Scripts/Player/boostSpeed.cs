using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem; // เพิ่มตัวนี้เข้ามา
using System.Collections;

public class boostSpeed : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    private NetworkVariable<float> currentVelocity = new NetworkVariable<float>();

    private bool isBoosting = false;
    private bool isCooldown = false;
    private Vector2 moveInput;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentVelocity.Value = walkSpeed;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Keyboard.current != null)
        {
            float x = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
            float y = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);
            moveInput = new Vector2(x, y);

            if (Keyboard.current.leftShiftKey.wasPressedThisFrame && !isBoosting && !isCooldown)
            {
                RequestBoostServerRpc();
            }
        }

        Move();
    }

    void Move()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        transform.position += move.normalized * currentVelocity.Value * Time.deltaTime;
    }

    [ServerRpc]
    void RequestBoostServerRpc()
    {
        if (!isBoosting && !isCooldown)
        {
            StartCoroutine(BoostTimer());
        }
    }

    IEnumerator BoostTimer()
    {
        isBoosting = true;
        currentVelocity.Value = runSpeed;

        yield return new WaitForSeconds(3f);

        currentVelocity.Value = walkSpeed;
        isBoosting = false;
        isCooldown = true;

        yield return new WaitForSeconds(5f);
        isCooldown = false;
    }
}