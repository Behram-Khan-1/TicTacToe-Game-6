using System;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action<int, int, PlayerType> ClickedOnGrid;
    public enum PlayerType { None, Circle, Cross };
    private PlayerType playerType;

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
    }
    public override void OnNetworkSpawn()
    {
        Debug.Log(NetworkManager.Singleton.LocalClientId);
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            playerType = PlayerType.Cross;
        }
        else
            playerType = PlayerType.Circle;
        
    }

    public void ClickedOnGridPosition(int x, int y)
    {
        Debug.Log("Clicked on grid position: " + x + ", " + y);
        ClickedOnGrid?.Invoke(x, y, playerType);
    }

    public PlayerType GetPlayerType()
    {
        return playerType;
    }
}
