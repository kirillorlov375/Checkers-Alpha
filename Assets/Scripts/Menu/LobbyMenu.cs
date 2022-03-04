using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] Button startGameButton;
    [SerializeField] Text[] playerNameTexts = new Text[2];

    void Start()
    {
        PlayerNetwork.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    void OnDestroy()
    {
        PlayerNetwork.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    void ClientHandleInfoUpdated()
    {
        List<PlayerNetwork> players = ((CheckersNetworkManager)NetworkManager.singleton).NetworkPlayers;
        for (int i = 0; i < players.Count; i++)
            playerNameTexts[i].text = players[i].DisplayName;
        for (int i = players.Count; i < playerNameTexts.Length; i++)
            playerNameTexts[i].text = "∆дем игрока...";
    }

    public void StartGame()
    {
        
    }
}
