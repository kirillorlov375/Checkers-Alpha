using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePromotionHandlerNetwork : PiecePromotionHandler
{
    public override void OnStartServer() {
        PieceMovementHandlerNetwork.OnPieceReachedBackline += TryPromotePiece;
    }

    public override void OnStopServer() {
        PieceMovementHandlerNetwork.OnPieceReachedBackline -= TryPromotePiece;
    }

    protected override bool TryPromotePiece(PiecePromotionHandler piecePromotionHandler, int x, int z) {
        bool promoted = base.TryPromotePiece(piecePromotionHandler, x, z);
        if(promoted) RpcPromotePiece();
        return promoted;
    }

    [ClientRpc]
    void RpcPromotePiece() {
        if(NetworkServer.active) return;
        PromotePiece();
    }
}
