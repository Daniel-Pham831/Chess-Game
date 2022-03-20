using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    protected override List<Vector2Int> GetAllPossibleMove()
    {
        List<Vector2Int> allPossibleMoveList = new List<Vector2Int>();

        Vector2Int forwardRightDirection = Vector2Int.one;
        AddedMoveRecursivelly(ref allPossibleMoveList, new Vector2Int(currentX, currentY) + forwardRightDirection, forwardRightDirection);

        Vector2Int backwardRightDirection = -Vector2Int.one;
        AddedMoveRecursivelly(ref allPossibleMoveList, new Vector2Int(currentX, currentY) + backwardRightDirection, backwardRightDirection);

        Vector2Int forwardLeftDirection = new Vector2Int(-1, 1);
        AddedMoveRecursivelly(ref allPossibleMoveList, new Vector2Int(currentX, currentY) + forwardLeftDirection, forwardLeftDirection);

        Vector2Int backwardLeftDirection = new Vector2Int(1, -1);
        AddedMoveRecursivelly(ref allPossibleMoveList, new Vector2Int(currentX, currentY) + backwardLeftDirection, backwardLeftDirection);

        return allPossibleMoveList;
    }

    protected override void AddedMoveRecursivelly(ref List<Vector2Int> allPossibleMoveList, Vector2Int checkMove, Vector2Int increament)
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
}