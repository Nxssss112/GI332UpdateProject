using Unity.Netcode;
using Unity.Collections;
using TMPro;
using UnityEngine;

public class JoinCodeUI : NetworkBehaviour
{
    [SerializeField] private TMP_Text joinCodeText;

    // ใช้ FixedString เพื่อให้ส่งค่า string ผ่าน Network ได้
    private NetworkVariable<FixedString32Bytes> networkJoinCode = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        // เมื่อค่าเปลี่ยน ให้ Update UI
        networkJoinCode.OnValueChanged += (oldValue, newValue) => {
            joinCodeText.text = newValue.ToString();
        };

        // ถ้าเป็น Host ให้กำหนดค่าเริ่มต้น
        if (IsServer)
        {
            string code = PlayerPrefs.GetString("JoinCode", "ERROR");
            networkJoinCode.Value = code;
        }
        else
        {
            // สำหรับ Client ที่เพิ่งเข้ามา ให้แสดงค่าปัจจุบันทันที
            joinCodeText.text = networkJoinCode.Value.ToString();
        }
    }
}