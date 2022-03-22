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
    [SerializeField] private Vector3 boardCenter = Vector3.zero;

    [Header("Prefabs & Materials")]
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private List<Material> teamMaterials;

    private const string tileLayer = "Tile";
    private const string hoverLayer = "Hover";
    private const string movableLayer = "Movable";

    // For logics
    public ChessPiece[,] chessPieces;
    private ChessPiece currentSelectedPiece;
    private DeadList deadList;
    private int smoothTime;

    // Player Turn
    private Team currentTurn;
    private Team playerTeam;
    private Team otherTeam;

    // For generateAllTiles
    private float tileSize;
    private float yOffset;
    private int TILE_COUNT_X;
    private int TILE_COUNT_Y;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;

    // For events
    private ChessBoardInputEvent chessBoardInputEvent;

    // For singleton
    public static ChessBoard Singleton { get; private set; }

    private ChessBoardConfiguration chessBoardConfiguration;

    private void Awake()
    {
        SetupSingleton();

        chessBoardInputEvent = GetComponent<ChessBoardInputEvent>();
        registerInputEvent(true);

        currentTurn = Team.Blue;
        playerTeam = Team.Blue;
        otherTeam = Team.Red;

        currentSelectedPiece = null;
        deadList = GetComponent<DeadList>();

        chessBoardConfiguration = ChessBoardConfiguration.Singleton;
    }

    private void Start()
    {
        InitializeValues();

        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();

        deadList.SetupDeadList(GetTileCenter(8, -1), GetTileCenter(-1, 8), tileSize, transform.forward);
    }

    private void SetupSingleton()
    {
        Singleton = this;
    }

    private void InitializeValues()
    {
        yOffset = ChessBoardConfiguration.Singleton.yOffset;
        tileSize = ChessBoardConfiguration.Singleton.tileSize;
        TILE_COUNT_X = ChessBoardConfiguration.Singleton.TILE_COUNT_X;
        TILE_COUNT_Y = ChessBoardConfiguration.Singleton.TILE_COUNT_Y;
        smoothTime = ChessBoardConfiguration.Singleton.smoothTime;
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
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask(tileLayer, hoverLayer, movableLayer)))
        {
            // Get the indexes of the hit tile
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer(hoverLayer);
            }

            // If we were already hovering a tile, change the previous one
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer(tileLayer);
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer(hoverLayer);
            }
        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer(tileLayer);
                currentHover = -Vector2Int.one;
            }
        }
    }

    public void ShowMovableOf(List<Vector2Int> movableList, bool reset = false)
    {
        foreach (Vector2Int movable in movableList)
        {
            if (!reset)
                tiles[movable.x, movable.y].layer = LayerMask.NameToLayer(movableLayer);
            else
                tiles[movable.x, movable.y].layer = LayerMask.NameToLayer(tileLayer);
        }
    }

    private void OnDestroy()
    {
        registerInputEvent(false);
    }

    // Input event handler
    private void OnLeftMouseButtonDown()
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
                if (CanCurrentSelectedPieceMoveHere(currentHover))
                {
                    //Debug.Log($"{currentSelectedPiece.pieceType.ToString()} killed {chessPieces[currentHover.x, currentHover.y].pieceType.ToString()}");
                    ReplaceHoverPieceWithCurrentSelectedPiece(true);
                }
            }
        }
        else
        {
            // If chessPiece at currentHover is not our piece
            if (chessPieces[currentHover.x, currentHover.y].team != playerTeam)
            {
                if (currentSelectedPiece == null) return;

                if (CanCurrentSelectedPieceMoveHere(currentHover))
                {
                    Debug.Log($"{currentSelectedPiece.pieceType.ToString()} killed {chessPieces[currentHover.x, currentHover.y].pieceType.ToString()}");
                    ReplaceHoverPieceWithCurrentSelectedPiece(true);

                    return;
                }
                else
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
            }
        }
    }

    private void ReplaceHoverPieceWithCurrentSelectedPiece(bool killConfirm = false)
    {
        currentSelectedPiece.Select();

        ChessPiece tempChessPiece = currentSelectedPiece;
        ChessPiece deadPiece = killConfirm ? chessPieces[currentHover.x, currentHover.y] : null;

        chessPieces[currentHover.x, currentHover.y] = currentSelectedPiece;
        chessPieces[tempChessPiece.currentX, tempChessPiece.currentY] = null;

        currentSelectedPiece = null;

        PositionASinglePiece(currentHover.x, currentHover.y);

        if (chessPieces[currentHover.x, currentHover.y].pieceType == ChessPieceType.Pawn)
            (chessPieces[currentHover.x, currentHover.y] as Pawn).hasMadeFirstMove = false;

        MoveDeadPieceToDeadList(deadPiece);
    }

    private void MoveDeadPieceToDeadList(ChessPiece deadPiece)
    {
        if (deadPiece == null) return;

        deadList.AddPieceToDeadList(deadPiece);
    }

    private bool CanCurrentSelectedPieceMoveHere(Vector2Int currentHover)
    {
        return chessPieces[currentSelectedPiece.currentX, currentSelectedPiece.currentY].IsMoveValid(currentHover);
    }

    private void registerInputEvent(bool confirm)
    {
        if (confirm)
        {
            chessBoardInputEvent.onLeftMouseButtonDown += OnLeftMouseButtonDown;
        }
        else
        {
            chessBoardInputEvent.onLeftMouseButtonDown -= OnLeftMouseButtonDown;
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

        tileObject.layer = LayerMask.NameToLayer(tileLayer);
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
                {
                    PositionASinglePiece(x, y, true);
                    chessPieces[x, y].yNormal = chessPieces[x, y].transform.position.y;
                    chessPieces[x, y].ySelected = chessPieces[x, y].transform.position.y * 2f;
                }
    }
    private void PositionASinglePiece(int x, int y, bool force = false)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        if (force)
            chessPieces[x, y].transform.position = GetTileCenter(x, y);
        else
        {
            StartCoroutine(SmoothPositionASinglePiece(chessPieces[x, y], GetTileCenter(x, y)));
        }
    }

    private IEnumerator SmoothPositionASinglePiece(ChessPiece cp, Vector3 targetPos)
    {
        for (float i = 0; i <= smoothTime; i++)
        {
            cp.transform.position = Vector3.Lerp(cp.transform.position, targetPos, i / smoothTime);
            yield return null;
        }
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
