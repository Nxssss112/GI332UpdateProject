using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

public struct PlayerScoreData : INetworkSerializable
{
    public FixedString32Bytes Name;
    public int Score;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref Score);
    }
}

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public static List<PlayerScoreData> FinalResults = new List<PlayerScoreData>();

    [Header("Game Settings")]
    public float startTime = 300f;
    public NetworkVariable<float> timeLeft = new NetworkVariable<float>();
    public NetworkVariable<bool> isGameRunning = new NetworkVariable<bool>();

    void Awake()
    {
        if (Instance == null) Instance = this;
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
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening) return;

        if (!isGameRunning.Value && NetworkManager.Singleton.ConnectedClientsList.Count >= 2)
        {
            isGameRunning.Value = true;
        }

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

    void StopGame()
    {
        if (!IsServer) return;
        isGameRunning.Value = false;

        CaptureAndSyncScores();

        NetworkManager.Singleton.SceneManager.LoadScene("Scoreboard", LoadSceneMode.Single);
    }

    private void CaptureAndSyncScores()
    {
        var allWallets = FindObjectsByType<TreasureWallet>(FindObjectsSortMode.None);
        List<PlayerScoreData> tempList = new List<PlayerScoreData>();

        foreach (var wallet in allWallets)
        {
            tempList.Add(new PlayerScoreData
            {
                Name = wallet.PlayerName.Value,
                Score = wallet.TotalTreasure.Value
            });
        }

        tempList = tempList.OrderByDescending(s => s.Score).ToList();

        SyncScoresClientRpc(tempList.ToArray());
    }

    [ClientRpc]
    private void SyncScoresClientRpc(PlayerScoreData[] results)
    {
        FinalResults = results.ToList();
    }
}