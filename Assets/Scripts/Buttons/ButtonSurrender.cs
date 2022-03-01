using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ButtonSurrender : MonoBehaviour
{
    public void Surrender()
    {
        if (LocalGameManager.Instance)
        {
            TurnsHandler.Instance.Surrender();
        }
        else
        {
            var surrenderingPlayer = NetworkClient.connection.identity.GetComponent<PlayerNetwork>();
            var players = ((CheckersNetworkManager)NetworkManager.singleton).NetworkPlayers;
            foreach (PlayerNetwork player in players)
                if (player != surrenderingPlayer)
                    surrenderingPlayer.CmdGameOver($"Победитель: {player.DisplayName}!");
        }
    }
}
