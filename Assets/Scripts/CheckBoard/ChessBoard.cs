using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

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
    private string capturableLayer = "Capturable";
    private List<string> layerList;

    // For logics
    public ChessPiece[,] chessPieces;
    private ChessPiece currentSelectedPiece;
    private ChessPiece nullPiece;
    private DeadList deadList;

    // Player Turn
    public Team currentTurn;

    // Multi logics
    private int playerCount = -1;
    public Team playerTeam;

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
    public event Action<Team> onTurnSwitched;
    public event Action<Team> onTeamVictory;
    public event Action<Team> onGameStart;

    // For singleton
    public static ChessBoard Singleton { get; private set; }
    private ChessBoardConfiguration chessBoardConfiguration;

    // Unity functions
    private void Awake()
    {
        this.SetupSingleton();

        registerEvents(true);

        this.currentTurn = Team.Blue;

        this.nullPiece = this.SpawnNullPiece();
        this.currentSelectedPiece = this.nullPiece;
        this.deadList = GetComponent<DeadList>();

        this.layerList = new List<string>(){
             this.tileLayer,
             this.hoverLayer,
             this.movableLayer,
             this.capturableLayer
        };

        this.chessBoardConfiguration = ChessBoardConfiguration.Singleton;
    }
    private void Start()
    {
        this.InitializeValues();

        this.GenerateAllTiles(this.tileSize, this.TILE_COUNT_X, this.TILE_COUNT_Y);
        this.SpawnAllPieces();
        this.PositionAllPieces();

        this.deadList.SetupDeadList(GetTileCenter(new Vector2Int(8, -1)), this.GetTileCenter(new Vector2Int(-1, 8)), this.tileSize, transform.forward);

        GameStateManager.Singleton.OnGameStateChanged += this.OnGameStateChanged;
        InputEventManager.Singleton.onLeftMouseButtonDown += this.OnLeftMouseButtonDown;
    }
    private void OnDestroy()
    {
        GameStateManager.Singleton.OnGameStateChanged -= this.OnGameStateChanged;
        InputEventManager.Singleton.onLeftMouseButtonDown -= this.OnLeftMouseButtonDown;

        this.registerEvents(false);
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
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask(this.layerList.ToArray())))
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

    // Init-Reset Functions
    private void OnGameStateChanged(GameState state, Turn turn)
    {
        switch (state)
        {
            case GameState.Reset:
                this.HandleResetState();
                break;
        }
    }

    private void HandleResetState()
    {
        this.currentSelectedPiece = this.nullPiece;
        List<ChessPiece> tempChessPieces = this.GetComponentsInChildren<ChessPiece>().ToList();

        foreach (ChessPiece piece in tempChessPieces)
        {
            Destroy(piece.gameObject);
        }

        this.SpawnAllPieces();
        this.PositionAllPieces();

        this.currentTurn = Team.Blue;

        this.onTurnSwitched?.Invoke(this.currentTurn);
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




    public void ShowMovableOf(List<Vector2Int> movableList, List<Vector2Int> capturableList, bool reset = false)
    {
        foreach (Vector2Int movable in movableList)
        {
            if (!reset)
                this.tiles[movable.x, movable.y].layer = LayerMask.NameToLayer(this.movableLayer);
            else
                this.tiles[movable.x, movable.y].layer = LayerMask.NameToLayer(this.tileLayer);
        }

        foreach (Vector2Int capturable in capturableList)
        {
            if (!reset)
                this.tiles[capturable.x, capturable.y].layer = LayerMask.NameToLayer(this.capturableLayer);
            else
                this.tiles[capturable.x, capturable.y].layer = LayerMask.NameToLayer(this.tileLayer);
        }
    }


    private void OnLeftMouseButtonDown()
    {
        // If this is not our turn
        if (this.currentTurn != this.playerTeam) return;

        // If the select outside of the board
        if (this.currentHover == -Vector2Int.one)
        {
            // If currentSelectedPiece is selected
            if (this.currentSelectedPiece.IsNotNull)
                this.currentSelectedPiece.SelectClient(-Vector2Int.one);

            return;
        }

        if (this.chessPieces[this.currentHover.x, this.currentHover.y].IsNull)
        {
            // If currentSelectedPiece is selected
            if (this.currentSelectedPiece.IsNotNull)
            {
                if (this.CanCurrentSelectedPieceMoveHere(this.currentHover))
                {
                    this.SendMovePieceToServer();
                }
            }
        }
        else
        {
            // If chessPiece at currentHover is not our team piece
            if (this.chessPieces[this.currentHover.x, this.currentHover.y].team != this.playerTeam)
            {
                if (this.currentSelectedPiece.IsNull) return;

                if (this.CanCurrentSelectedPieceMoveHere(this.currentHover))
                {
                    if (this.chessPieces[this.currentHover.x, this.currentHover.y].pieceType == ChessPieceType.King)
                        Client.Singleton.SendToServer(new NetVictoryClaim(this.currentTurn));

                    Debug.Log($"{this.currentSelectedPiece.pieceType.ToString()} killed {this.chessPieces[this.currentHover.x, this.currentHover.y].pieceType.ToString()}");

                    this.SendMovePieceToServer(KillConfirm.Kill);

                    return;
                }
            }
            else
            {
                if (this.currentSelectedPiece.IsNotNull)
                {
                    this.currentSelectedPiece.SelectClient(-Vector2Int.one);
                }
                else
                {
                    this.currentSelectedPiece.SelectClient(this.currentHover);
                }
            }
        }
    }


    // For Handling chess piece movement of the chess board
    private void ReplaceHoverPieceWithCurrentSelectedPiece(KillConfirm killConfirm = KillConfirm.Move)
    {
        ChessPiece tempChessPiece = this.currentSelectedPiece;
        ChessPiece deadPiece = killConfirm == KillConfirm.Kill ? this.chessPieces[this.currentHover.x, this.currentHover.y] : this.nullPiece;

        this.chessPieces[this.currentHover.x, this.currentHover.y] = this.currentSelectedPiece;
        this.chessPieces[tempChessPiece.currentX, tempChessPiece.currentY] = this.nullPiece;


        this.chessPieces[this.currentHover.x, this.currentHover.y].MoveTo(this.currentHover);

        this.SwitchTurn();

        this.MoveDeadPieceToDeadList(deadPiece);
    }
    private void MoveDeadPieceToDeadList(ChessPiece deadPiece)
    {
        if (deadPiece.IsNull) return;

        this.deadList.AddPieceToDeadList(deadPiece);
    }
    private bool CanCurrentSelectedPieceMoveHere(Vector2Int currentHover)
    {
        return this.chessPieces[this.currentSelectedPiece.currentX, this.currentSelectedPiece.currentY].IsMoveValid(currentHover);
    }
    public void SwitchTurn()
    {
        if (this.currentTurn == Team.Blue)
        {
            this.currentTurn = Team.Red;
        }
        else
        {
            this.currentTurn = Team.Blue;
        }

        this.onTurnSwitched?.Invoke(this.currentTurn);
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

        // Spawn blue team pieces
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

        // Spawn red team pieces
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


        // Spawn null pieces
        for (int x = 0; x < this.TILE_COUNT_X; x++)
        {
            for (int y = 0; y < this.TILE_COUNT_Y; y++)
            {
                if (this.chessPieces[x, y] == null)
                    this.chessPieces[x, y] = this.SpawnNullPiece();
            }
        }
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
    private ChessPiece SpawnNullPiece()
    {
        return Instantiate(this.prefabs[(int)ChessPieceType.NullPiece], transform).GetComponent<ChessPiece>();
    }

    // Position
    private void PositionAllPieces()
    {
        for (int x = 0; x < this.TILE_COUNT_X; x++)
            for (int y = 0; y < this.TILE_COUNT_Y; y++)
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

    // Input event handler
    // confirm means subscript
    private void registerEvents(bool confirm)
    {
        if (confirm)
        {
            //   InputEventManager.Singleton.onLeftMouseButtonDown += this.OnLeftMouseButtonDown;
            //Server
            NetUtility.S_WELCOME += this.OnWelcomeServer;
            NetUtility.S_PIECE_SELECTED += this.OnPieceSelectedServer;
            NetUtility.S_MAKE_MOVE += this.OnMakeMoveServer;
            NetUtility.S_VICTORY_CLAIM += this.OnVictoryClaimServer;

            //Client
            NetUtility.C_WELCOME += this.OnWelcomeClient;
            NetUtility.C_START_GAME += this.OnStartGameClient;
            NetUtility.C_PIECE_SELECTED += this.OnPieceSelectedClient;
            NetUtility.C_MAKE_MOVE += this.OnMakeMoveClient;
            NetUtility.C_VICTORY_CLAIM += this.OnVictoryClaimClient;
            NetUtility.C_SWITCH_TEAM += onNetSwitchTeamClient;
        }
        else
        {
            //   InputEventManager.Singleton.onLeftMouseButtonDown -= this.OnLeftMouseButtonDown;
            //Server
            NetUtility.S_WELCOME -= this.OnWelcomeServer;
            NetUtility.S_PIECE_SELECTED -= this.OnPieceSelectedServer;
            NetUtility.S_MAKE_MOVE -= this.OnMakeMoveServer;
            NetUtility.S_VICTORY_CLAIM -= this.OnVictoryClaimServer;

            //Client
            NetUtility.C_WELCOME -= this.OnWelcomeClient;
            NetUtility.C_START_GAME -= this.OnStartGameClient;
            NetUtility.C_PIECE_SELECTED -= this.OnPieceSelectedClient;
            NetUtility.C_MAKE_MOVE -= this.OnMakeMoveClient;
            NetUtility.C_VICTORY_CLAIM -= this.OnVictoryClaimClient;
            NetUtility.C_SWITCH_TEAM -= onNetSwitchTeamClient;
        }
    }





    #region NetworkSendingMessage

    private void SendMovePieceToServer(KillConfirm killConfirm = KillConfirm.Move)
    {
        Client.Singleton.SendToServer(new NetMakeMove(this.currentHover.x, this.currentHover.y, killConfirm));
    }

    #endregion

    #region Network Received Message
    // Server
    private void OnWelcomeServer(NetMessage message, NetworkConnection connectedClient)
    {
        // At this point, there is a client has connected to the server
        // We need to assign a team and return the message back to that client
        NetWelcome netWelcome = message as NetWelcome;

        netWelcome.AssignedTeam = this.AssignTeamToClient(++this.playerCount);

        Server.Singleton.SendToClient(connectedClient, netWelcome);

        if (this.playerCount == 1)
            Server.Singleton.BroadCast(new NetStartGame());
    }

    private Team AssignTeamToClient(int currentTotalUser)
    {
        return currentTotalUser == 0 ? Team.Blue : Team.Red;
    }

    private void OnPieceSelectedServer(NetMessage netMessage, NetworkConnection sender)
    {
        NetPieceSelected netPieceSelected = netMessage as NetPieceSelected;

        Server.Singleton.BroadCast(netPieceSelected);
    }

    private void OnMakeMoveServer(NetMessage netMessage, NetworkConnection sender)
    {
        NetMakeMove netMakeMove = netMessage as NetMakeMove;

        Server.Singleton.BroadCast(netMakeMove);
    }

    private void OnVictoryClaimServer(NetMessage netMessage, NetworkConnection sender)
    {
        NetVictoryClaim netVictoryClaim = netMessage as NetVictoryClaim;

        Server.Singleton.BroadCast(netVictoryClaim);
    }


    //Client
    private void OnWelcomeClient(NetMessage message)
    {
        NetWelcome netWelcome = message as NetWelcome;

        this.playerTeam = netWelcome.AssignedTeam;

        Debug.Log($"My team is {this.playerTeam}");
    }

    private void OnStartGameClient(NetMessage message)
    {
        this.onGameStart?.Invoke(this.playerTeam);
    }

    private void OnPieceSelectedClient(NetMessage message)
    {
        NetPieceSelected netPieceSelected = message as NetPieceSelected;

        if (this.currentSelectedPiece.IsNotNull)
        {
            this.currentSelectedPiece.SetPieceSelect();
            this.currentSelectedPiece = this.nullPiece;
        }
        else
        {
            this.currentSelectedPiece = this.chessPieces[netPieceSelected.currentX, netPieceSelected.currentY];
            this.currentSelectedPiece.SetPieceSelect();
        }
    }

    private void OnMakeMoveClient(NetMessage message)
    {
        NetMakeMove netMakeMove = message as NetMakeMove;

        this.currentHover.x = netMakeMove.NextX;
        this.currentHover.y = netMakeMove.NextY;

        this.ReplaceHoverPieceWithCurrentSelectedPiece(netMakeMove.killConfirm);

        this.currentSelectedPiece.SetPieceSelect();
        this.currentSelectedPiece = this.nullPiece;
    }

    private void OnVictoryClaimClient(NetMessage netMessage)
    {
        NetVictoryClaim netVictoryClaim = netMessage as NetVictoryClaim;

        GameStateManager.Singleton.UpdateGameState(GameState.Victory, (Turn)netVictoryClaim.VictoryTeam);
    }

    private void onNetSwitchTeamClient(NetMessage netMessage)
    {
        this.currentTurn = Team.Blue;
        this.playerTeam = this.playerTeam == Team.Blue ? Team.Red : Team.Blue;
    }
    #endregion
}
