using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceNetwork : NetworkBehaviour
{
    [SyncVar]
    PlayerPiecesHandlerNetworked owner;

    public override void OnStartServer()
    {
        owner = connectionToClient.identity.GetComponent<PlayerPiecesHandlerNetworked>();
        Board.Instance.OnPieceCaptured += ServerHandlePieceCaptured;
    }

    public override void OnStopServer()
    {
        Board.Instance.OnPieceCaptured -= ServerHandlePieceCaptured;
    }

    [Server]
    void ServerHandlePieceCaptured(Vector3 capturedPiecePosition)
    {
        if (capturedPiecePosition != transform.position) return;
        NetworkServer.Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(SetParent());
    }

    IEnumerator SetParent()
    {
        while (!transform.parent)
        {
            transform.parent = owner.PiecesParent;
            yield return null;
        }
    }
}
