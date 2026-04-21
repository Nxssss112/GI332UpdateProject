using Unity.Netcode;
using UnityEngine;

public class TreasureWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalTreasure = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void OnTriggerEnter(Collider col)
    {
        if (!IsServer) return;
        if (NetworkManager.Singleton == null) return;
        if (NetworkManager.Singleton.ConnectedClientsList.Count < 2) return;

        if (!col.TryGetComponent<Treasure>(out Treasure treasure)) return;

        int value = treasure.Collect();

        if (value > 0)
        {
            TotalTreasure.Value += value;
        }
    }
}