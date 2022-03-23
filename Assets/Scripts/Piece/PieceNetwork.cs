using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceNetwork : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleOwnerSet))]
    PlayerPiecesHandler owner;

    public override void OnStartServer()
    {
        owner = connectionToClient.identity.GetComponent<PlayerPiecesHandler>();
    }

    void HandleOwnerSet(PlayerPiecesHandler oldOwner, PlayerPiecesHandler newOwner)
    {
        transform.parent = newOwner.PiecesParent;
    }
}
