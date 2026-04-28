using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class boostSpeed : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("Energy Settings")]
    public float maxEnergy = 3f;
    public float regenSpeed = 1f;
    public float drainSpeed = 1f;

    [Header("UI")]
    public Image boostBarFill;

    private NetworkVariable<float> currentVelocity = new NetworkVariable<float>();

    private float currentEnergy;
    private bool isBoosting = false;

    private Vector2 moveInput;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentVelocity.Value = walkSpeed;
        }

        currentEnergy = maxEnergy;
    }

    void Update()
    {
        if (!IsOwner) return;

        HandleInput();
        HandleBoost();
        Move();
        UpdateUI();
    }

    void HandleInput()
    {
        if (Keyboard.current == null) return;

        float x = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
        float y = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);
        moveInput = new Vector2(x, y);

        // ?? กดครั้งเดียว = เริ่มพุ่ง (ต้องเต็มก่อน)
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && !isBoosting && currentEnergy >= maxEnergy)
        {
            isBoosting = true;
        }
    }

    void HandleBoost()
    {
        if (isBoosting)
        {
            currentEnergy -= drainSpeed * Time.deltaTime;

            if (currentEnergy <= 0f)
            {
                currentEnergy = 0f;
                isBoosting = false;
            }
        }
        else
        {
            if (currentEnergy < maxEnergy)
            {
                currentEnergy += regenSpeed * Time.deltaTime;
            }
        }

        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        // ?? ให้ Server เป็นคนกำหนดความเร็ว
        UpdateSpeedServerRpc(isBoosting);
    }

    [ServerRpc]
    void UpdateSpeedServerRpc(bool boosting)
    {
        currentVelocity.Value = boosting ? runSpeed : walkSpeed;
    }

    void Move()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        transform.position += move.normalized * currentVelocity.Value * Time.deltaTime;
    }

    void UpdateUI()
    {
        if (boostBarFill == null) return;

        boostBarFill.fillAmount = currentEnergy / maxEnergy;
    }
}