using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckersNetworkManager : NetworkManager
{
    [SerializeField] GameObject gameOverHandlerPrefab, boardPrefab, 
        turnsHandlerPrefab;
    public List<PlayerNetwork> NetworkPlayers = new List<PlayerNetwork>();
    public List<Player> Players = new List<Player>();

    public static event Action ClientOnConnected;
    public static event Action ServerOnGameStarted;

    public override void OnStartServer()
    {
        var boardInstance = Instantiate(boardPrefab);
        NetworkServer.Spawn(boardInstance);
        var turnsHandlerInstance = Instantiate(turnsHandlerPrefab);
        NetworkServer.Spawn(turnsHandlerInstance);
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

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject playerInstance = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, playerInstance);
        var player = playerInstance.GetComponent<PlayerNetwork>();
        NetworkPlayers.Add(player);
        Players.Add(player);
        player.LobbyOwner = player.IsWhite = numPlayers == 1;
        player.DisplayName = player.IsWhite ? "Светлый" : "Тёмный";
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        var player = conn.identity.GetComponent<PlayerNetwork>();
        NetworkPlayers.Remove(player);
        Players.Remove(player);
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        NetworkPlayers.Clear();
        Players.Clear();
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        SceneManager.LoadScene("Lobby Scene");
        Destroy(gameObject);
    }
}
