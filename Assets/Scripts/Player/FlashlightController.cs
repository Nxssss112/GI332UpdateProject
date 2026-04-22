using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class FlashlightController : NetworkBehaviour
{
    public Light flashlightLight;

    // ใช้ ReadPermission เพื่อให้ทุกคนอ่านได้ และ WritePermission ให้เฉพาะ Server เขียนได้
    private NetworkVariable<bool> isOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        // บังคับให้ปิดไฟตอนเริ่ม
        if (flashlightLight != null)
            flashlightLight.enabled = isOn.Value;

        // ดักจับการเปลี่ยนแปลงค่า
        isOn.OnValueChanged += OnFlashlightChanged;
    }

    private void OnFlashlightChanged(bool previousValue, bool newValue)
    {
        if (flashlightLight != null)
        {
            flashlightLight.enabled = newValue;
            Debug.Log("Flashlight status changed to: " + newValue);
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        // ลองใช้สไตล์นี้เพื่อเช็คปุ่ม F
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log("F Key Pressed!"); // ถ้ากดแล้ว log นี้ไม่ขึ้น แสดงว่า Input System มีปัญหา
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