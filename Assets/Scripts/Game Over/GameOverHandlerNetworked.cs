using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameOverHandlerNetworked : GameOverHandler
{
    public override void OnStartServer() {
        TurnsHandler.Instance.OnGameOver += GameOverHandler;
    }

    public override void OnStopServer() {
        TurnsHandler.Instance.OnGameOver -= GameOverHandler;
    }

    [ClientRpc]
    void RpcGameOver(string result) {
        CallGameOver(result);
    }

    [Server]
    void GameOverHandler(string result) {
        RpcGameOver(result);
    }
}
