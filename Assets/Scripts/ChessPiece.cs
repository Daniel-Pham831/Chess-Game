using System.Collections;
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

    public bool IsSelected { get; private set; }
    public float yNormal;
    public float ySelected;

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
}
