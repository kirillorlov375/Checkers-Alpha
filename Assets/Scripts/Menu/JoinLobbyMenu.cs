using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] GameObject onlinePage;
    [SerializeField] InputField addressInput;
    [SerializeField] Button joinButton;

    void OnEnable()
    {
        CheckersNetworkManager.ClientOnConnected += HandleClientConnected;
        CheckersNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
    }

    void OnDisable()
    {
        CheckersNetworkManager.ClientOnConnected -= HandleClientConnected;
        CheckersNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }

    public void Join()
    {
        string address = addressInput.text;
        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();
        joinButton.interactable = false;
    }

    void HandleClientConnected()
    {
        joinButton.interactable = true;
        gameObject.SetActive(false);
        onlinePage.SetActive(false);
    }

    void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
