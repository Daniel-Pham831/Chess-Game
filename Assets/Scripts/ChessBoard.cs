using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Blue = 0,
    Red = 1
}

public class ChessBoard : MonoBehaviour
{
    [Header("Art Section")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;

    [Header("Prefabs & Materials")]
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private List<Material> teamMaterials;

    // For logics
    private ChessPiece[,] chessPieces;
    private ChessPiece currentSelectedPiece;

    // Player Turn
    private Team currentTurn;
    private Team playerTeam;
    private Team otherTeam;

    // For generateAllTiles
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;

    // For events
    private ChessBoardInputEvent chessBoardInputEvent;

    private void Awake()
    {
        chessBoardInputEvent = GetComponent<ChessBoardInputEvent>();
        registerInputEvent(true);

        currentTurn = Team.Blue;
        playerTeam = Team.Blue;
        otherTeam = Team.Red;

        currentSelectedPiece = null;

        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover")))
        {
            // Get the indexes of the hit tile
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we were already hovering a tile, change the previous one
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
        }
    }

    private void OnDestroy()
    {
        registerInputEvent(false);
    }

    // Input event handler
    private void OnLeftMouseButtonDown()
    {
        // If player click out of the board, cancel the move
        if (currentHover == -Vector2Int.one)
        {
            if (currentSelectedPiece != null)
                currentSelectedPiece.Select();
            currentSelectedPiece = null;
            return;
        }

        // If this is our turn
        if (true)
        {
            // If that position has a valid chess piece then selected
            if (chessPieces[currentHover.x, currentHover.y] != null)
            {
                chessPieces[currentHover.x, currentHover.y].Select();
                currentSelectedPiece = chessPieces[currentHover.x, currentHover.y];
            }
        }
    }

    private void OnLeftMouseButtonDown1()
    {
        // If this is not our turn
        if (currentTurn != playerTeam) return;

        // If the select outside of the board
        if (currentHover == -Vector2Int.one)
        {
            // If currentSelectedPiece is selected
            if (currentSelectedPiece != null)
                currentSelectedPiece.Select();

            currentSelectedPiece = null;

            return;
        }

        if (chessPieces[currentHover.x, currentHover.y] == null)
        {
            // If currentSelectedPiece is selected
            if (currentSelectedPiece != null)
            {
                currentSelectedPiece.MoveTo(currentHover);
                currentSelectedPiece.Select();
                currentSelectedPiece = null;
            }
        }
        else
        {
            // If chess at currentHover is not our piece
            if (chessPieces[currentHover.x, currentHover.y].team != playerTeam)
            {
                if (currentSelectedPiece == null) return;

                if (CanCurrentSelectedPieceMoveHere(currentHover))
                {
                    ReplaceThisPieceWithCurrentSelectedPiece(chessPieces[currentHover.x, currentHover.y]);
                    Debug.Log($"{currentSelectedPiece.pieceType.ToString()} killed {chessPieces[currentHover.x, currentHover.y].pieceType.ToString()}");

                    currentSelectedPiece = null;
                    return;
                }

                Debug.Log($"{currentSelectedPiece.pieceType.ToString()} Cannot kill {chessPieces[currentHover.x, currentHover.y].pieceType.ToString()}");
            }
            else
            {
                if (currentSelectedPiece != null)
                {
                    currentSelectedPiece.Select();
                    currentSelectedPiece = null;
                }
                else
                {
                    currentSelectedPiece = chessPieces[currentHover.x, currentHover.y];
                    currentSelectedPiece.Select();
                }
                return;
            }
        }
    }

    private void ReplaceThisPieceWithCurrentSelectedPiece(ChessPiece thisPiece)
    {
        // replace the current hover piece with the current selected piece
        thisPiece = currentSelectedPiece; // sai
    }

    private bool CanCurrentSelectedPieceMoveHere(Vector2Int currentHover)
    {
        return true;
    }

    private void registerInputEvent(bool comfirm)
    {
        if (comfirm)
        {
            chessBoardInputEvent.onLeftMouseButtonDown += OnLeftMouseButtonDown1;
        }
        else
        {
            chessBoardInputEvent.onLeftMouseButtonDown -= OnLeftMouseButtonDown1;
        }
    }

    // Generate the board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountY / 2) * tileSize) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    // Spawning of the pieces
    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        // Spawn team1 pieces
        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, Team.Blue);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, Team.Blue);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, Team.Blue);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.Queen, Team.Blue);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.King, Team.Blue);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, Team.Blue);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, Team.Blue);
        chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, Team.Blue);
        for (int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i, 1] = SpawnSinglePiece(ChessPieceType.Pawn, Team.Blue);

        // Spawn team1 pieces
        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, Team.Red);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, Team.Red);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, Team.Red);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.Queen, Team.Red);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.King, Team.Red);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, Team.Red);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, Team.Red);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, Team.Red);
        for (int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, Team.Red);
    }
    private ChessPiece SpawnSinglePiece(ChessPieceType pieceType, Team team)
    {
        ChessPiece cp = Instantiate(prefabs[(int)pieceType], transform).GetComponent<ChessPiece>();

        cp.pieceType = pieceType;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[(int)cp.team];

        if (cp.team == Team.Red)
            cp.transform.Rotate(Vector3.up, -180);

        return cp;
    }

    // Position
    private void PositionAllPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPieces[x, y] != null)
                    PositionASinglePiece(x, y, true);
    }
    private void PositionASinglePiece(int x, int y, bool force = false)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x, y].transform.position = GetTileCenter(x, y);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    // Operations
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one;
    }

}
