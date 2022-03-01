using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : Player
{
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public static event Action ClientOnInfoUpdated;

    [SyncVar]
    ulong steamId;
    public ulong SteamId
    {
        get { return steamId; }
        [Server]
        set 
        {
            steamId = value;
            var cSteamId = new CSteamID(steamId);
            DisplayName = SteamFriends.GetFriendPersonaName(cSteamId);
        }
    }

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))] 
    string displayName;
    public string DisplayName 
    {
        get { return displayName; }
        [Server]
        set { displayName = value; }
    }

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerUpdated))] 
    bool partyOwner;
    public bool PartyOwner
    {
        get { return partyOwner; }
        [Server]
        set { partyOwner = value; }
    }

    #region Server
    public override void OnStartServer()
    {
        DontDestroyOnLoad(gameObject);
    }

    [Command]
    public void CmdStartGame()
    {
        if (!PartyOwner) return;
        ((CheckersNetworkManager)NetworkManager.singleton).StartGame();
    }

    [Command]
    public void CmdNextTurn()
    {
        TurnsHandler.Instance.NextTurn();
    }

    [Command]
    public void CmdGameOver(string result)
    {
        TurnsHandler.Instance.Surrender(result);
    }
    #endregion

    #region Client
    public override void OnStartClient()
    {
        if (NetworkServer.active) return;
        ((CheckersNetworkManager)NetworkManager.singleton).NetworkPlayers.Add(this);
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();
        if (!isClientOnly) return;
        ((CheckersNetworkManager)NetworkManager.singleton).NetworkPlayers.Remove(this);
    }

    void ClientHandleDisplayNameUpdated(string old, string newDisplayName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    void AuthorityHandlePartyOwnerUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) return;
        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }
    #endregion
}
