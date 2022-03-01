using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] GameObject lobbyUI;
    [SerializeField] Button startGameButton;
    [SerializeField] Text[] playerNameTexts = new Text[2];

    void Start()
    {
        CheckersNetworkManager.ClientOnConnected += HandleClientConnected;
        PlayerNetwork.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        PlayerNetwork.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    void OnDestroy()
    {
        CheckersNetworkManager.ClientOnConnected -= HandleClientConnected;
        PlayerNetwork.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        PlayerNetwork.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    void ClientHandleInfoUpdated()
    {
        var players = ((CheckersNetworkManager)NetworkManager.singleton).NetworkPlayers;
        for (int i = 0; i < players.Count; i++)
            playerNameTexts[i].text = players[i].DisplayName;
        for (int i = players.Count; i < playerNameTexts.Length; i++)
            playerNameTexts[i].text = "Waiting for player...";
        startGameButton.interactable = players.Count >= 2;
    }

    void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }

    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<PlayerNetwork>().CmdStartGame();
    }
}
