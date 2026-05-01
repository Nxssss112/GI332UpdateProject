using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : IDisposable
{
    private Allocation allocation;
    private string joinCode;
    private string lobbyId;

    private NetworkServer networkServer;
    private Coroutine heartbeatCoroutine;

    private const int MaxConnections = 20;
    private const string GameSceneName = "Game";
    private const string JoinCodeKey = "JoinCode";

    public async Task StartHostAsync(bool isPrivate)
    {
        // Relay Allocation
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        // Get Join Code
        try
        {
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("Join Code: " + joinCode);

            PlayerPrefs.SetString(JoinCodeKey, joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        // Setup Transport
        UnityTransport transport =
            NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData =
            allocation.ToRelayServerData("dtls");

        transport.SetRelayServerData(relayServerData);

        // Create Lobby
        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();

            // ใช้ค่าที่ส่งเข้ามา
            lobbyOptions.IsPrivate = isPrivate;

            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode",
                    new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: joinCode
                    )
                }
            };

            string playerName =
                PlayerPrefs.GetString(
                    NameSelector.PlayerNameKey,
                    "Unknown"
                );

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(
                $"{playerName}'s Lobby",
                MaxConnections,
                lobbyOptions
            );

            lobbyId = lobby.Id;

            // Start Heartbeat
            heartbeatCoroutine =
                HostSingleton.Instance.StartCoroutine(
                    HeartbeatLobby(15)
                );
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return;
        }

        // Create Network Server
        networkServer = new NetworkServer(NetworkManager.Singleton);

        // Player Data
        UserData userData = new UserData
        {
            userName = PlayerPrefs.GetString(
                NameSelector.PlayerNameKey,
                "Missing Name"
            ),

            userAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);

        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        // Start Host
        NetworkManager.Singleton.StartHost();

        // Load Game Scene
        NetworkManager.Singleton.SceneManager.LoadScene(
            GameSceneName,
            LoadSceneMode.Single
        );
    }

    private IEnumerator HeartbeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay =
            new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);

            yield return delay;
        }
    }

    public async void Dispose()
    {
        // Stop Heartbeat
        if (heartbeatCoroutine != null)
        {
            HostSingleton.Instance.StopCoroutine(heartbeatCoroutine);
        }

        // Delete Lobby
        if (!string.IsNullOrEmpty(lobbyId))
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

            lobbyId = string.Empty;
        }

        // Dispose Network Server
        networkServer?.Dispose();
    }
}