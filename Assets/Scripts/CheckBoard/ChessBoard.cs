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

    private string tileLayer = "Tile";
    private string hoverLayer = "Hover";
    private string movableLayer = "Movable";

    // For logics
    public ChessPiece[,] chessPieces;
    private ChessPiece currentSelectedPiece;
    private DeadList deadList;

    // Player Turn
    public Team currentTurn;
    public Team playerTeam;
    public Team otherTeam;

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
    public event Action onTurnSwitched;

    // For singleton
    public static ChessBoard Singleton { get; private set; }
    private ChessBoardConfiguration chessBoardConfiguration;


    private void Awake()
    {
        this.SetupSingleton();

        this.chessBoardInputEvent = GetComponent<ChessBoardInputEvent>();
        registerInputEvent(true);

        this.currentTurn = Team.Blue;
        this.playerTeam = Team.Blue;
        this.otherTeam = Team.Red;

        this.currentSelectedPiece = null;
        this.deadList = GetComponent<DeadList>();

        this.chessBoardConfiguration = ChessBoardConfiguration.Singleton;
    }

    private void Start()
    {
        this.InitializeValues();

        GenerateAllTiles(this.tileSize, this.TILE_COUNT_X, this.TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();

        this.deadList.SetupDeadList(GetTileCenter(new Vector2Int(8, -1)), GetTileCenter(new Vector2Int(-1, 8)), this.tileSize, transform.forward);
    }

    private void SetupSingleton()
    {
        Singleton = this;
    }

    private void InitializeValues()
    {
        this.yOffset = ChessBoardConfiguration.Singleton.yOffset;
        this.tileSize = ChessBoardConfiguration.Singleton.tileSize;
        this.TILE_COUNT_X = ChessBoardConfiguration.Singleton.TILE_COUNT_X;
        this.TILE_COUNT_Y = ChessBoardConfiguration.Singleton.TILE_COUNT_Y;
    }


    private void Update()
    {
        if (!this.currentCamera)
        {
            this.currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = this.currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask(this.tileLayer, this.hoverLayer, this.movableLayer)))
        {
            // Get the indexes of the hit tile
            Vector2Int hitPosition = this.LookupTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (this.currentHover == -Vector2Int.one)
            {
                this.currentHover = hitPosition;
                this.tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer(this.hoverLayer);
            }

            // If we were already hovering a tile, change the previous one
            if (this.currentHover != -Vector2Int.one)
            {
                this.tiles[this.currentHover.x, this.currentHover.y].layer = LayerMask.NameToLayer(this.tileLayer);
                this.currentHover = hitPosition;
                this.tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer(this.hoverLayer);
            }
        }
        else
        {
            if (this.currentHover != -Vector2Int.one)
            {
                this.tiles[this.currentHover.x, this.currentHover.y].layer = LayerMask.NameToLayer(this.tileLayer);
                this.currentHover = -Vector2Int.one;
            }
        }
    }

    public void ShowMovableOf(List<Vector2Int> movableList, bool reset = false)
    {
        foreach (Vector2Int movable in movableList)
        {
            if (!reset)
                this.tiles[movable.x, movable.y].layer = LayerMask.NameToLayer(this.movableLayer);
            else
                this.tiles[movable.x, movable.y].layer = LayerMask.NameToLayer(this.tileLayer);
        }
    }

    private void OnDestroy()
    {
        this.registerInputEvent(false);
    }

    // Input event handler
    private void OnLeftMouseButtonDown()
    {
        // If this is not our turn
        if (this.currentTurn != this.playerTeam) return;

        // If the select outside of the board
        if (this.currentHover == -Vector2Int.one)
        {
            // If currentSelectedPiece is selected
            if (this.currentSelectedPiece != null)
                this.currentSelectedPiece.Select();

            this.currentSelectedPiece = null;

            return;
        }

        if (this.chessPieces[this.currentHover.x, this.currentHover.y] == null)
        {
            // If currentSelectedPiece is selected
            if (this.currentSelectedPiece != null)
            {
                if (this.CanCurrentSelectedPieceMoveHere(this.currentHover))
                {
                    this.ReplaceHoverPieceWithCurrentSelectedPiece();
                }
            }
        }
        else
        {
            // If chessPiece at currentHover is not our piece
            if (this.chessPieces[this.currentHover.x, this.currentHover.y].team != this.playerTeam)
            {
                if (this.currentSelectedPiece == null) return;

                if (this.CanCurrentSelectedPieceMoveHere(this.currentHover))
                {
                    Debug.Log($"{this.currentSelectedPiece.pieceType.ToString()} killed {this.chessPieces[this.currentHover.x, this.currentHover.y].pieceType.ToString()}");
                    this.ReplaceHoverPieceWithCurrentSelectedPiece(true);

                    return;
                }
                else
                    Debug.Log($"{this.currentSelectedPiece.pieceType.ToString()} Cannot kill {this.chessPieces[this.currentHover.x, this.currentHover.y].pieceType.ToString()}");
            }
            else
            {
                if (this.currentSelectedPiece != null)
                {
                    this.currentSelectedPiece.Select();
                    this.currentSelectedPiece = null;
                }
                else
                {
                    this.currentSelectedPiece = this.chessPieces[this.currentHover.x, this.currentHover.y];
                    this.currentSelectedPiece.Select();
                }
            }
        }
    }

    private void ReplaceHoverPieceWithCurrentSelectedPiece(bool killConfirm = false)
    {
        this.currentSelectedPiece.Select();

        ChessPiece tempChessPiece = this.currentSelectedPiece;
        ChessPiece deadPiece = killConfirm ? this.chessPieces[this.currentHover.x, this.currentHover.y] : null;

        this.chessPieces[this.currentHover.x, this.currentHover.y] = this.currentSelectedPiece;
        this.chessPieces[tempChessPiece.currentX, tempChessPiece.currentY] = null;

        this.currentSelectedPiece = null;

        this.chessPieces[this.currentHover.x, this.currentHover.y].MoveTo(this.currentHover);

        this.SwitchTurn();

        this.MoveDeadPieceToDeadList(deadPiece);
    }

    private void MoveDeadPieceToDeadList(ChessPiece deadPiece)
    {
        if (deadPiece == null) return;

        this.deadList.AddPieceToDeadList(deadPiece);
    }

    private bool CanCurrentSelectedPieceMoveHere(Vector2Int currentHover)
    {
        return this.chessPieces[this.currentSelectedPiece.currentX, this.currentSelectedPiece.currentY].IsMoveValid(currentHover);
    }

    private void registerInputEvent(bool confirm)
    {
        if (confirm)
        {
            this.chessBoardInputEvent.onLeftMouseButtonDown += this.OnLeftMouseButtonDown;
        }
        else
        {
            this.chessBoardInputEvent.onLeftMouseButtonDown -= this.OnLeftMouseButtonDown;
        }
    }

    // Generate the board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        this.yOffset += transform.position.y;
        this.bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountY / 2) * tileSize) + this.boardCenter;

        this.tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                this.tiles[x, y] = this.GenerateSingleTile(tileSize, x, y);
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = this.tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, this.yOffset, y * tileSize) - this.bounds;
        vertices[1] = new Vector3(x * tileSize, this.yOffset, (y + 1) * tileSize) - this.bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, this.yOffset, y * tileSize) - this.bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, this.yOffset, (y + 1) * tileSize) - this.bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer(this.tileLayer);
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    // Spawning of the pieces
    private void SpawnAllPieces()
    {
        this.chessPieces = new ChessPiece[this.TILE_COUNT_X, this.TILE_COUNT_Y];

        // Spawn team1 pieces
        this.chessPieces[0, 0] = this.SpawnSinglePiece(ChessPieceType.Rook, Team.Blue);
        this.chessPieces[1, 0] = this.SpawnSinglePiece(ChessPieceType.Knight, Team.Blue);
        this.chessPieces[2, 0] = this.SpawnSinglePiece(ChessPieceType.Bishop, Team.Blue);
        this.chessPieces[3, 0] = this.SpawnSinglePiece(ChessPieceType.Queen, Team.Blue);
        this.chessPieces[4, 0] = this.SpawnSinglePiece(ChessPieceType.King, Team.Blue);
        this.chessPieces[5, 0] = this.SpawnSinglePiece(ChessPieceType.Bishop, Team.Blue);
        this.chessPieces[6, 0] = this.SpawnSinglePiece(ChessPieceType.Knight, Team.Blue);
        this.chessPieces[7, 0] = this.SpawnSinglePiece(ChessPieceType.Rook, Team.Blue);
        for (int i = 0; i < this.TILE_COUNT_X; i++)
            this.chessPieces[i, 1] = this.SpawnSinglePiece(ChessPieceType.Pawn, Team.Blue);

        // Spawn team1 pieces
        this.chessPieces[0, 7] = this.SpawnSinglePiece(ChessPieceType.Rook, Team.Red);
        this.chessPieces[1, 7] = this.SpawnSinglePiece(ChessPieceType.Knight, Team.Red);
        this.chessPieces[2, 7] = this.SpawnSinglePiece(ChessPieceType.Bishop, Team.Red);
        this.chessPieces[3, 7] = this.SpawnSinglePiece(ChessPieceType.Queen, Team.Red);
        this.chessPieces[4, 7] = this.SpawnSinglePiece(ChessPieceType.King, Team.Red);
        this.chessPieces[5, 7] = this.SpawnSinglePiece(ChessPieceType.Bishop, Team.Red);
        this.chessPieces[6, 7] = this.SpawnSinglePiece(ChessPieceType.Knight, Team.Red);
        this.chessPieces[7, 7] = this.SpawnSinglePiece(ChessPieceType.Rook, Team.Red);
        for (int i = 0; i < this.TILE_COUNT_X; i++)
            this.chessPieces[i, 6] = this.SpawnSinglePiece(ChessPieceType.Pawn, Team.Red);
    }
    private ChessPiece SpawnSinglePiece(ChessPieceType pieceType, Team team)
    {
        ChessPiece cp = Instantiate(this.prefabs[(int)pieceType], transform).GetComponent<ChessPiece>();

        cp.pieceType = pieceType;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = this.teamMaterials[(int)cp.team];

        if (cp.team == Team.Red)
            cp.transform.Rotate(Vector3.up, -180);

        return cp;
    }

    // Position
    private void PositionAllPieces()
    {
        for (int x = 0; x < this.TILE_COUNT_X; x++)
            for (int y = 0; y < this.TILE_COUNT_Y; y++)
                if (this.chessPieces[x, y] != null)
                {
                    this.chessPieces[x, y].MoveTo(new Vector2Int(x, y), true);
                    this.chessPieces[x, y].yNormal = this.chessPieces[x, y].transform.position.y;
                    this.chessPieces[x, y].ySelected = this.chessPieces[x, y].transform.position.y * 2f;
                }
    }

    public Vector3 GetTileCenter(Vector2Int position)
    {
        return new Vector3(position.x * this.tileSize, this.yOffset, position.y * this.tileSize) - this.bounds + new Vector3(this.tileSize / 2, 0, this.tileSize / 2);
    }

    // Operations
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < this.TILE_COUNT_X; x++)
            for (int y = 0; y < this.TILE_COUNT_Y; y++)
                if (this.tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one;
    }

    public void SwitchTurn()
    {
        if (this.currentTurn == Team.Blue)
        {
            this.currentTurn = Team.Red;
            this.playerTeam = Team.Red;
            this.otherTeam = Team.Blue;
        }
        else
        {
            this.currentTurn = Team.Blue;
            this.playerTeam = Team.Blue;
            this.otherTeam = Team.Red;
        }

        this.onTurnSwitched?.Invoke();
    }

}
