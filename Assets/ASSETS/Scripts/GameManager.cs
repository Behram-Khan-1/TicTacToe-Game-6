using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action<int, int, PlayerType> ClickedOnGrid;
    public event Action OnGameStarted;
    public event Action OnTurnChange;
    public event Action<Line, PlayerType> OnGameWin;
    public enum PlayerType { None, Circle, Cross };
    private PlayerType playerType;
    private NetworkVariable<PlayerType> currentPlayerTurn = new NetworkVariable<PlayerType>();

    private PlayerType[,] playerTypeArray;

    public enum Orientation
    {
        Horizontal,
        Vertical,
        DiagonalA,
        DiagonalB,

    }

    private List<Line> lines;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        playerTypeArray = new PlayerType[3, 3];


        lines = new List<Line>()
        {
            //Horizontal Lines
            new Line{
                gridVector2Int = new List<Vector2Int> { new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(0,2) },
                centerGridVector = new Vector2Int(0,1),
                orientation = Orientation.Horizontal
            },
            new Line{
                gridVector2Int = new List<Vector2Int> { new Vector2Int(1,0), new Vector2Int(1,1), new Vector2Int(1,2) },
                centerGridVector = new Vector2Int(1,1),
                orientation = Orientation.Horizontal
            },
            new Line{
                gridVector2Int = new List<Vector2Int> { new Vector2Int(2,0), new Vector2Int(2,1), new Vector2Int(2,2) },
                centerGridVector = new Vector2Int(2,1),
                orientation = Orientation.Horizontal
            },
            //Vertical Lines
            new Line{
                gridVector2Int = new List<Vector2Int> { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0) },
                centerGridVector = new Vector2Int(0,1),
                orientation = Orientation.Vertical
            },
            new Line{
                gridVector2Int = new List<Vector2Int> { new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(2,1) },
                centerGridVector = new Vector2Int(1,1),
                orientation = Orientation.Vertical
            },
            new Line{
                gridVector2Int = new List<Vector2Int> { new Vector2Int(0,2), new Vector2Int(1,2), new Vector2Int(2,2) },
                centerGridVector = new Vector2Int(2,1),
                orientation = Orientation.Vertical
            },
            //Diagonal Lines
            new Line{
                gridVector2Int = new List<Vector2Int> { new Vector2Int(0,0), new Vector2Int(1,1), new Vector2Int(2,2) },
                centerGridVector = new Vector2Int(1,1),
                orientation = Orientation.DiagonalA
            },
            new Line{
                gridVector2Int = new List<Vector2Int> { new Vector2Int(0,2), new Vector2Int(1,1), new Vector2Int(2,0) },
                centerGridVector = new Vector2Int(1,1),
                orientation = Orientation.DiagonalB
            },
        };


    }
    public override void OnNetworkSpawn()
    {
        Debug.Log(NetworkManager.Singleton.LocalClientId);
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            playerType = PlayerType.Circle;
        }
        else
            playerType = PlayerType.Cross;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        currentPlayerTurn.OnValueChanged += OnCurrentPlayerTurnChanged;
    }
    void OnCurrentPlayerTurnChanged(PlayerType previousValue, PlayerType newValue)
    {
        OnTurnChange?.Invoke();
    }


    public void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            currentPlayerTurn.Value = PlayerType.Circle;
            TriggerOnGameStartRPC();
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartRPC()
    {
        OnGameStarted?.Invoke();

    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRPC(int x, int y, PlayerType playerType)
    {
        if (playerType != currentPlayerTurn.Value)
        {
            return;
        }

        if (playerTypeArray[x, y] != PlayerType.None)
        {
            return;
        }

        playerTypeArray[x, y] = playerType;

        Debug.Log("Clicked on grid position: " + x + ", " + y);
        ClickedOnGrid?.Invoke(x, y, playerType);

        switch (currentPlayerTurn.Value)
        {
            case PlayerType.Circle:
                currentPlayerTurn.Value = PlayerType.Cross;
                break;
            case PlayerType.Cross:
                currentPlayerTurn.Value = PlayerType.Circle;
                break;
        }

        TestWinnerFunction();
    }

    private bool TestWinnerLine(Line line)
    {
        return TestWin(
            playerTypeArray[line.gridVector2Int[0].x, line.gridVector2Int[0].y],
            playerTypeArray[line.gridVector2Int[1].x, line.gridVector2Int[1].y],
            playerTypeArray[line.gridVector2Int[2].x, line.gridVector2Int[2].y]
        );

    }

    private bool TestWin(PlayerType a, PlayerType b, PlayerType c)
    {
        return a == b && b == c && a != PlayerType.None;
    }

    public void TestWinnerFunction()
    {
        foreach (Line line in lines)
        {
            if (TestWinnerLine(line))
            {
                Debug.Log("Win");
                currentPlayerTurn.Value = PlayerType.None;
                OnGameWin?.Invoke(line, playerTypeArray[line.centerGridVector.x, line.centerGridVector.y]);
                break;
            }
        }
    }


    public PlayerType GetPlayerType()
    {
        return playerType;
    }
    public PlayerType GetCurrentPlayer()
    {
        return currentPlayerTurn.Value;
    }


    public struct Line
    {
        public List<Vector2Int> gridVector2Int;
        public Vector2Int centerGridVector;
        public Orientation orientation;
    }
}
