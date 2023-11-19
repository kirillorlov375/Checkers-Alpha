using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovementHandlerNetwork : PieceMovementHandler
{
    public static event Func<PiecePromotionHandler, int, int, bool> OnPieceReachedBackline;

    public override void OnStartAuthority()
    {
        TilesSelectionHandler.OnTileSelected += HandleTileSelected;
    }

    public override void OnStopClient()
    {
        TilesSelectionHandler.OnTileSelected -= HandleTileSelected;
    }

    protected override void Move(Vector3 position, bool nextTurn)
    {
        CmdMove(position, nextTurn);
    }

    protected override void ReachedBackline(Vector2Int newPosition) {
        OnPieceReachedBackline?.Invoke(promotionHandler, newPosition.x, newPosition.y);
    }

    [Command]
    void CmdMove(Vector3 position, bool nextTurn)
    {
        base.Move(position, nextTurn);
    }

    protected override void Capture(Vector2Int piecePosition)
    {
        CmdCapture(piecePosition);
    }

    [Command]
    void CmdCapture(Vector2Int piecePosition)
    {
        base.Capture(piecePosition);
    }

    [ClientRpc]
    void RpcPlayAudio() {
        base.PlayAudio();
    }

    protected override void PlayAudio() {
        RpcPlayAudio();
    }
}
