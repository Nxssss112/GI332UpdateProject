using UnityEngine;
using Unity.Netcode;

public class DestroySelfOnContact : MonoBehaviour
{
    private ulong shooterId;

    public void SetOwner(ulong id) => shooterId = id;

    private void OnTriggerEnter(Collider col)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        SubmarineController target = col.GetComponentInParent<SubmarineController>();

        if (target != null)
        {
            // 1. ถ้าชนตัวเอง ให้ปล่อยผ่าน
            if (target.OwnerClientId == shooterId) return;

            // 2. สั่ง Stun ผู้เล่นที่โดนยิง
            target.ApplyHit();

            // 3. แจ้งเตือนคนยิง (Hit Feedback)
            ClientRpcParams rpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { shooterId }
                }
            };
            target.NotifyHitSuccessClientRpc(rpcParams);

            Despawn();
        }
        else
        {
            // ชนกำแพงหรือสิ่งของอื่นๆ
            Despawn();
        }
    }

    private void Despawn()
    {
        if (TryGetComponent<NetworkObject>(out var netObj))
            netObj.Despawn();
        else
            Destroy(gameObject);
    }
}