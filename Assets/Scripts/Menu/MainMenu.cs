using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject landingPagePanel, onlinePage, lobbyParent;
    public static bool UseSteam { get; private set; } = true;
    Callback<LobbyCreated_t> lobbyCreated;
    Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    Callback<LobbyEnter_t> lobbyEntered;
    public static CSteamID LobbyID { get; private set; }

    void Start() {
        if(!UseSteam) return;
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    void OnDisable() {
        if(!UseSteam || lobbyCreated == null) return;
        lobbyCreated.Dispose();
        gameLobbyJoinRequested.Dispose();
        lobbyEntered.Dispose();
    }

    public void HostLobby()
    {
        if (UseSteam) SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 2);
        else NetworkManager.singleton.StartHost();
    }

    void OnLobbyCreated(LobbyCreated_t callback) {
        if(!UseSteam) return;
        if(callback.m_eResult != EResult.k_EResultOK) {
            landingPagePanel.SetActive(true);
            return;
        }
        LobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(
            LobbyID,
            "HostAddress",
            SteamUser.GetSteamID().ToString()
        );
        NetworkManager.singleton.StartHost();
    }

    void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback) {
        if(!UseSteam) return;

        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    void OnLobbyEntered(LobbyEnter_t callback) {
        if(!UseSteam) return;
        if(NetworkServer.active) return;

        LobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        string hostAddress = SteamMatchmaking.GetLobbyData(
            LobbyID,
            "HostAddress"
        );
        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();
        landingPagePanel.SetActive(false);
        onlinePage.SetActive(false);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
            Application.Quit();
        #endif
    }
}
