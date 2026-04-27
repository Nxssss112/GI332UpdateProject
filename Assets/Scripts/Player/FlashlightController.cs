using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class FlashlightController : NetworkBehaviour
{
    public Light flashlightLight;

    private NetworkVariable<bool> isOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (flashlightLight != null)
            flashlightLight.enabled = isOn.Value;

        isOn.OnValueChanged += OnFlashlightChanged;
    }

    private void OnFlashlightChanged(bool previousValue, bool newValue)
    {
        if (flashlightLight != null)
        {
            flashlightLight.enabled = newValue;
            //Debug.Log("Flashlight status changed to: " + newValue);
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            //Debug.Log("F Key Pressed!"); // ถ้ากดแล้ว log นี้ไม่ขึ้น แสดงว่า Input System มีปัญหา
            ToggleFlashlightServerRpc();
        }
    }

    [ServerRpc]
    void ToggleFlashlightServerRpc()
    {
        isOn.Value = !isOn.Value;
    }

    public override void OnNetworkDespawn()
    {
        isOn.OnValueChanged -= OnFlashlightChanged;
    }
}