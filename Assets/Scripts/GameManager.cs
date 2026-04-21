using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public NetworkVariable<float> timeLeft = new NetworkVariable<float>(300f);
    public NetworkVariable<bool> isGameRunning = new NetworkVariable<bool>(false);

    void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            timeLeft.Value = 300f;
            isGameRunning.Value = false;
        }
    }

    void Update()
    {
        if (!IsServer) return; // นับเวลาเฉพาะฝั่ง Server

        // กัน null สำคัญมาก
        if (NetworkManager.Singleton == null) return;
        if (!NetworkManager.Singleton.IsListening) return;

        // เริ่มเกมถ้ามีผู้เล่น >= 2
        if (!isGameRunning.Value && NetworkManager.Singleton.ConnectedClientsList.Count >= 2)
        {
            isGameRunning.Value = true;
        }

        // นับเวลา
        if (isGameRunning.Value)
        {
            timeLeft.Value -= Time.deltaTime;

            if (timeLeft.Value <= 0)
            {
                timeLeft.Value = 0;
                isGameRunning.Value = false;
                StopGame();
            }
        }
    }

    void StopGame()
    {
        // เรียก RPC ให้ทุก Client หยุดเวลา
        StopGameClientRpc();
    }

    [ClientRpc]
    void StopGameClientRpc()
    {
        Time.timeScale = 0f; // หยุดเกมทุก Client
        Debug.Log("Game Over!");
    }
}