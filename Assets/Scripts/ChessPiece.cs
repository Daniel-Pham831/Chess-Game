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

public class ChessPiece : MonoBehaviour
{
    public ChessPieceType pieceType;
    public Team team;
    public int currentX;
    public int currentY;

    private Vector3 desiredPosition;
    private Vector3 desiredScale;

}
