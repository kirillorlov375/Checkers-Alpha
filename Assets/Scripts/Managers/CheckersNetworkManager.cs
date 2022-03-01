using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckersNetworkManager : NetworkManager
{
    [SerializeField] GameObject gameOverHandlerPrefab, boardPrefab, turnsHandlerPrefab;

    public List<PlayerNetwork> NetworkPlayers { get; } = new List<PlayerNetwork>();
    public List<Player> Players { get; } = new List<Player>();
    bool gameInProgress = false;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;
    public static event Action ServerOnGameStarted;

    #region Server
    public override void OnStartServer()
    {
        var boardInstance = Instantiate(boardPrefab);
        NetworkServer.Spawn(boardInstance);
        var turnsHandlerInstance = Instantiate(turnsHandlerPrefab);
        NetworkServer.Spawn(turnsHandlerInstance);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (gameInProgress) conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        var player = conn.identity.GetComponent<PlayerNetwork>();
        NetworkPlayers.Remove(player);
        Players.Remove(player);
        base.OnServerDisconnect(conn);
        if (networkSceneName == "Game Scene")
        {
            StopHost();
            Destroy(gameObject);
        }
    }

    public override void OnStopServer()
    {
        NetworkPlayers.Clear();
        Players.Clear();
        gameInProgress = false;
        if (MainMenu.UseSteam)
            SteamMatchmaking.LeaveLobby(MainMenu.LobbyID);
    }

    public void StartGame()
    {
        if (NetworkPlayers.Count < 2) return;
        gameInProgress = true;
        ServerChangeScene("Game Scene");
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject playerInstance = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, playerInstance);
        var player = playerInstance.GetComponent<PlayerNetwork>();
        NetworkPlayers.Add(player);
        Players.Add(player);
        
        player.PartyOwner = player.IsWhite = numPlayers == 1;

        if (MainMenu.UseSteam)
        {
            CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(
                MainMenu.LobbyID, numPlayers - 1);
            player.SteamId = steamId.m_SteamID;
        }
        else
        {
            player.DisplayName = player.IsWhite? "Светлый" : "Тёмный";
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith("Game"))
        {
            ServerOnGameStarted?.Invoke();
            var gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
            NetworkServer.Spawn(gameOverHandlerInstance);
        }
    }
    #endregion

    #region Client
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        ClientOnDisconnected?.Invoke();
        if (NetworkPlayers.Count == 0)
        {
            SceneManager.LoadScene(0);
            Destroy(gameObject);
        }
        if (MainMenu.UseSteam)
            SteamMatchmaking.LeaveLobby(MainMenu.LobbyID);
    }
    #endregion
}
