using Unity.Netcode;
using Unity.Collections;
using UnityEngine;
using System.Collections;

public class TreasureWallet : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>(
        "", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public NetworkVariable<int> TotalTreasure = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            StartCoroutine(WaitAndSetName());
        }
    }

    private IEnumerator WaitAndSetName()
    {
        yield return new WaitForSeconds(0.2f);
        string savedName = PlayerPrefs.GetString("PlayerName", "Unknown");
        UpdateNameServerRpc(savedName);
    }

    [ServerRpc]
    private void UpdateNameServerRpc(string name)
    {
        PlayerName.Value = name;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!IsServer) return;
        if (col.TryGetComponent<Treasure>(out Treasure treasure))
        {
            int value = treasure.Collect();
            if (value > 0)
            {
                TotalTreasure.Value += value;
            }
        }
    }
}