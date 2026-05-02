using Unity.Netcode;
using Unity.Collections;
using UnityEngine;
using System.Collections;

public class TreasureWallet : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> PlayerName =
        new NetworkVariable<FixedString32Bytes>(
            "",
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<int> TotalTreasure =
        new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectSound;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            StartCoroutine(WaitAndSetName());
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

                // ?? เล่นเสียงให้ “คนที่เก็บได้ยิน”
                PlayCollectSoundClientRpc(OwnerClientId);
            }
        }
    }

    // ?? ส่งไปเฉพาะคนที่เก็บ
    [ClientRpc]
    private void PlayCollectSoundClientRpc(ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
            return;

        if (audioSource != null && collectSound != null)
            audioSource.PlayOneShot(collectSound);
    }
}