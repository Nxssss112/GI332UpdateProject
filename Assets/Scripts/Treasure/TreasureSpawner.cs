using Unity.Netcode;
using UnityEngine;

public class TreasureSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int spawnAmount = 20;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        SpawnTreasures(); // ?? เรียกตรงนี้
    }

    // ?? เอาโค้ดที่คุณส่งมาใส่ตรงนี้เลย
    void SpawnTreasures()
    {
        int count = Mathf.Min(spawnAmount, spawnPoints.Length);

        for (int i = 0; i < count; i++)
        {
            if (spawnPoints[i] == null)
            {
                Debug.LogError("SpawnPoint " + i + " is NULL");
                continue;
            }

            GameObject treasure = Instantiate(
                treasurePrefab,
                spawnPoints[i].position,
                Quaternion.identity
            );

            NetworkObject netObj = treasure.GetComponent<NetworkObject>();

            if (netObj == null)
            {
                Debug.LogError("No NetworkObject on Treasure!");
                continue;
            }

            netObj.Spawn();
        }
    }
}