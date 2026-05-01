using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class Treasure : NetworkBehaviour
{
    [Header("Treasure")]
    [SerializeField] private Renderer render;

    [Header("Minimap")]
    [SerializeField] private Renderer minimapIcon;

    [SerializeField] private float respawnTime = 30f;

    private int treasureValue = 1;

    private NetworkVariable<bool> isCollected = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public int Collect()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.isGameRunning.Value)
        {
            return 0;
        }

        if (isCollected.Value)
            return 0;

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
        bool gameStarted =
            GameManager.Instance != null &&
            GameManager.Instance.isGameRunning.Value;

        bool visible = gameStarted && !isCollected.Value;

        // ตัวของจริง
        render.enabled = visible;

        // icon minimap
        minimapIcon.enabled = visible;
    }
}