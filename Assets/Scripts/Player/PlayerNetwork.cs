using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : Player
{
    public static event Action ClientOnInfoUpdated;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    string displayName;
    public string DisplayName
    {
        get { return displayName; }
        [Server]
        set { displayName = value; }
    }

    void ClientHandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    public override void OnStartClient()
    {
        if (!isClientOnly)
            return;
        ((CheckersNetworkManager)NetworkManager.singleton).NetworkPlayers.Add(this);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly)
            ((CheckersNetworkManager)NetworkManager.singleton).NetworkPlayers.Remove(this);
        ClientOnInfoUpdated?.Invoke();
    }
}
