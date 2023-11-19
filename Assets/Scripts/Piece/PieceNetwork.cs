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
        Board.Instance.OnPieceCaptured += HandleServerPieceCaptured;
    }

    public override void OnStopServer() {
        Board.Instance.OnPieceCaptured -= HandleServerPieceCaptured;
    }

    void HandleOwnerSet(PlayerPiecesHandler oldOwner, PlayerPiecesHandler newOwner)
    {
        transform.parent = newOwner.PiecesParent;
    }

    [Server]
    void HandleServerPieceCaptured(Vector3 capturePiecePosition) {
        if(capturePiecePosition != transform.position) return;
        NetworkServer.Destroy(gameObject);
    }
}
