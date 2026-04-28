using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float startTime = 300f;

    public NetworkVariable<float> timeLeft = new NetworkVariable<float>();
    public NetworkVariable<bool> isGameRunning = new NetworkVariable<bool>();

    void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            timeLeft.Value = startTime;
            isGameRunning.Value = false;
        }
    }

    void Update()
    {
        if (!IsServer) return;

        // กัน null
        if (NetworkManager.Singleton == null) return;
        if (!NetworkManager.Singleton.IsListening) return;

        // เริ่มเกมเมื่อมีผู้เล่น >= 2
        if (!isGameRunning.Value && NetworkManager.Singleton.ConnectedClientsList.Count >= 2)
        {
            StartGame();
        }

        // นับเวลา
        if (isGameRunning.Value)
        {
            timeLeft.Value -= Time.deltaTime;

            if (timeLeft.Value <= 0)
            {
                timeLeft.Value = 0;
                StopGame();
            }
        }
    }

    void StartGame()
    {
        timeLeft.Value = startTime;
        isGameRunning.Value = true;

        Debug.Log("Game Started");
    }

    void StopGame()
    {
        if (!IsServer) return;

        isGameRunning.Value = false;

        Debug.Log("Game Over!");

        // โหลด Scene Scoreboard (sync ทุก client)
        NetworkManager.Singleton.SceneManager.LoadScene("Scoreboard", LoadSceneMode.Single);
    }
}