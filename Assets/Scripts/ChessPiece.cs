using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType
{
    None = -1,
    Pawn = 0,
    Rook = 1,
    Knight = 2,
    Bishop = 3,
    Queen = 4,
    King = 5,
}

public abstract class ChessPiece : MonoBehaviour
{
    public ChessPieceType pieceType;
    public Team team;
    public int currentX;
    public int currentY;

    public bool IsSelected { get; private set; }
    public float yNormal;
    public float ySelected;

    protected List<Vector2Int> validMoveList;

    private void Awake()
    {
        validMoveList = new List<Vector2Int>();
    }

    public void Select()
    {
        if (!IsSelected)
        {
            IsSelected = true;
        }
        else
        {
            IsSelected = false;
        }

        transform.position = new Vector3(transform.position.x, IsSelected ? ySelected : yNormal, transform.position.z);
    }

    public bool IsMoveValid(Vector2Int targetMove)
    {
        foreach (Vector2Int validMove in validMoveList)
        {
            if (validMove == targetMove) return true;
        }

        return false;
    }

    public void UpdateValidMoveList()
    {
        validMoveList = GetAllPossibleMove();
    }

    protected abstract List<Vector2Int> GetAllPossibleMove();

    protected virtual void AddedMoveRecursivelly(ref List<Vector2Int> allPossibleMoveList, Vector2Int checkMove, Vector2Int increament)
    {
        if (IsOutsideTheBoard(checkMove))
            return;

        if (IsBeingBlockedByTeamAt(checkMove)) return;
        if (IsBeingBlockedByOtherTeamAt(checkMove))
        {
            allPossibleMoveList.Add(checkMove);
            return;
        }

        allPossibleMoveList.Add(checkMove);

        AddedMoveRecursivelly(ref allPossibleMoveList, checkMove + increament, increament);
    }

    protected bool IsOutsideTheBoard(Vector2Int targetMove)
    {
        if (targetMove.x >= ChessBoardConfiguration.Singleton.TILE_COUNT_X || targetMove.x < 0
        || targetMove.y >= ChessBoardConfiguration.Singleton.TILE_COUNT_Y || targetMove.y < 0)
            return true;

        return false;
    }

    protected bool IsInsideTheBoard(Vector2Int targetMove)
    {
        return !IsOutsideTheBoard(targetMove);
    }

    protected virtual bool IsBeingBlockedAt(Vector2Int targetMove)
    {
        if (IsOutsideTheBoard(targetMove)) return true; // If outside the board then count as being blocked

        return ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y] != null ? true : false;
    }

    protected virtual bool IsBeingBlockedByTeamAt(Vector2Int targetMove)
    {
        if (IsOutsideTheBoard(targetMove)) return true;

        return ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y] != null ?
            (ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y].team == team ? true : false)
                : false;
    }

    protected virtual bool IsBeingBlockedByOtherTeamAt(Vector2Int targetMove)
    {
        return ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y] != null ?
            (ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y].team != team ? true : false)
                : false;
    }
}
