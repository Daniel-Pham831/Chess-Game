using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType
{
    NullPiece = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6,
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

    [HideInInspector] public bool isBeingAttackedByBlue;
    [HideInInspector] public bool isBeingAttackedByRed;

    protected List<Vector2Int> validMoveList;

    // For null checking
    public bool IsNull => this.pieceType == ChessPieceType.NullPiece ? true : false;
    public bool IsNotNull => !this.IsNull;

    protected virtual void Awake()
    {
        this.validMoveList = new List<Vector2Int>();

        this.Reset();
    }

    protected virtual void Start() { }

    private void Update()
    {
        if (this.IsSelected)
            ChessBoard.Singleton.ShowMovableOf(this.validMoveList);
    }

    public void Select()
    {
        if (!this.IsSelected)
        {
            this.IsSelected = true;
            UpdateValidMoveList();
        }
        else
        {
            this.IsSelected = false;
            ChessBoard.Singleton.ShowMovableOf(this.validMoveList, true);
        }

        this.transform.position = new Vector3(this.transform.position.x, this.IsSelected ? this.ySelected : this.yNormal, this.transform.position.z);
    }

    public bool IsMoveValid(Vector2Int targetMove)
    {
        foreach (Vector2Int validMove in this.validMoveList)
        {
            if (validMove == targetMove) return true;
        }

        return false;
    }

    public void UpdateValidMoveList()
    {
        this.validMoveList = GetAllPossibleMove();

        ChessPiece[,] chessPieces = ChessBoard.Singleton.chessPieces;
        foreach (Vector2Int validMove in this.validMoveList)
        {
            chessPieces[validMove.x, validMove.y].SetIsBeingAttacked(this.team, true);
        }
    }

    protected abstract List<Vector2Int> GetAllPossibleMove();

    protected void Reset()
    {
        this.isBeingAttackedByBlue = false;
        this.isBeingAttackedByRed = false;
    }

    public void SetIsBeingAttacked(Team attackerTeam, bool value)
    {
        if (attackerTeam == Team.Blue)
            isBeingAttackedByBlue = value;
        else
            isBeingAttackedByRed = value;
    }

    public virtual void MoveTo(Vector2Int targetMove, bool force = false)
    {
        this.currentX = targetMove.x;
        this.currentY = targetMove.y;

        Vector3 tileCenter = ChessBoard.Singleton.GetTileCenter(targetMove);

        if (force)
            this.transform.position = tileCenter;
        else
        {
            StartCoroutine(this.SmoothPositionASinglePiece(tileCenter));
        }
    }

    private IEnumerator SmoothPositionASinglePiece(Vector3 targetPos)
    {
        int smoothTime = ChessBoardConfiguration.Singleton.smoothTime;
        for (float i = 0; i <= smoothTime; i++)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetPos, i / smoothTime);
            yield return null;
        }
    }

    protected virtual void AddedMoveRecursivelly(ref List<Vector2Int> allPossibleMoveList, Vector2Int checkMove, Vector2Int increament)
    {
        if (this.IsOutsideTheBoard(checkMove))
            return;

        if (this.IsBeingBlockedByTeamAt(checkMove)) return;
        if (this.IsBeingBlockedByOtherTeamAt(checkMove))
        {
            allPossibleMoveList.Add(checkMove);
            return;
        }

        allPossibleMoveList.Add(checkMove);

        this.AddedMoveRecursivelly(ref allPossibleMoveList, checkMove + increament, increament);
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
        return !this.IsOutsideTheBoard(targetMove);
    }

    protected virtual bool IsBeingBlockedAt(Vector2Int targetMove)
    {
        if (this.IsOutsideTheBoard(targetMove)) return true; // If outside the board then count as being blocked

        return ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y].IsNotNull ? true : false;
    }

    protected virtual bool IsBeingBlockedByTeamAt(Vector2Int targetMove)
    {
        if (this.IsOutsideTheBoard(targetMove)) return true;

        return ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y].IsNotNull ?
            (ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y].team == this.team ? true : false)
                : false;
    }

    protected virtual bool IsBeingBlockedByOtherTeamAt(Vector2Int targetMove)
    {
        return ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y].IsNotNull ?
            (ChessBoard.Singleton.chessPieces[targetMove.x, targetMove.y].team != this.team ? true : false)
                : false;
    }
}
