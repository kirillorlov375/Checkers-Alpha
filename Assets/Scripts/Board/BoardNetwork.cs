using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardNetwork : Board
{
    readonly SyncList<int[]> boardList = new SyncList<int[]>();
    public override IList<int[]> BoardList { get { return boardList; } }
    public override event Action<Vector3> OnPieceCaptured;

    public override void OnStartServer()
    {
        FillBoardList(boardList);
        PieceMovementHandlerNetwork.OnPieceReachedBackline += TryPromotePieceBoard;
    }

    public override void OnStopServer() {
        PieceMovementHandlerNetwork.OnPieceReachedBackline -= TryPromotePieceBoard;
    }

    [Server]
    public override void MoveOnBoard(Vector2Int oldPosition, Vector2Int newPosition, bool nextTurn)
    {
        MoveOnBoard(boardList, oldPosition, newPosition);
        RpcMoveOnBoard(oldPosition, newPosition, nextTurn);
    }

    [ClientRpc]
    void RpcMoveOnBoard(Vector2Int oldPosition, Vector2Int newPosition, bool nextTurn)
    {
        if (NetworkServer.active)
            return;
        MoveOnBoard(boardList, oldPosition, newPosition);
        if (nextTurn)
            NetworkClient.connection.identity.GetComponent<PlayerNetwork>().CmdNextTurn();
    }

    [Server]
    public override void CaptureOnBoard(Vector2Int piecePosition) {
        Capture(boardList, piecePosition);
        RpcCaptureOnBoard(piecePosition);
        OnPieceCaptured?.Invoke(new Vector3(piecePosition.x, 0, piecePosition.y));
    }

    [ClientRpc]
    void RpcCaptureOnBoard(Vector2Int piecePosition) {
        Capture(boardList, piecePosition);
    }

    [Server]
    bool TryPromotePieceBoard(PiecePromotionHandler promotionHandler, int x, int z) {
        PromotePieceOnBoard(boardList, x, z);
        RpcPromotePieceOnBoard(x, z);
        return true;
    }

    [ClientRpc]
    void RpcPromotePieceOnBoard(int x, int z) {
        if(NetworkServer.active) return;
        PromotePieceOnBoard(boardList, x, z);
    }
}
