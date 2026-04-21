using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class Treasure : NetworkBehaviour
{
    [SerializeField] private Renderer render;
    [SerializeField] private float respawnTime = 20f;

    private int treasureValue = 1;

    private NetworkVariable<bool> isCollected = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public int Collect()
    {
        if (isCollected.Value) return 0;

        isCollected.Value = true;

        if (IsServer)
        {
            StartCoroutine(RespawnRoutine());
        }

        return treasureValue;
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);
        isCollected.Value = false;
    }

    void Update()
    {
        render.enabled = !isCollected.Value;
    }
}