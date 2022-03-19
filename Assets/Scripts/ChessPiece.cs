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

    private Vector3 desiredPosition;
    private Vector3 desiredScale;
    private float closeDistance = 0.1f;

    public bool IsSelected { get; private set; }

    public void Select()
    {
        if (!IsSelected)
        {
            IsSelected = true;
            StartCoroutine(MovePiece(IsSelected));
        }
        else
        {
            IsSelected = false;
            StartCoroutine(MovePiece(IsSelected));
        }
    }

    private IEnumerator MovePiece(bool up)
    {
        Vector3 desiredUpPosition = transform.position += (up ? Vector3.up : -Vector3.up) * 2f;
        while ((transform.position - desiredUpPosition).sqrMagnitude < closeDistance * closeDistance)
        {
            transform.position = Vector3.Lerp(transform.position, desiredUpPosition, Time.deltaTime * 10f);
            yield return null;
        }
    }
}
