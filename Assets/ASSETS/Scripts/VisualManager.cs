using Unity.Netcode;
using UnityEngine;

public class VisualManager : NetworkBehaviour
{
    [SerializeField] private Transform crossPrefeb;
    [SerializeField] private Transform circlePrefab;
    [SerializeField] private Transform greenWinLine;
    private const float GRIDSIZE = 3f;
    void Start()
    {
        GameManager.Instance.ClickedOnGrid += ClickedOnGridVisual;
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
    }

    void GameManager_OnGameWin(GameManager.Line line, GameManager.PlayerType playerType)
    {
        float zAngle = 0f;
        switch (line.orientation)
        {
            case GameManager.Orientation.Horizontal:
                zAngle = 0f;
                break;
            case GameManager.Orientation.Vertical:
                zAngle = 90f;
                break;
            case GameManager.Orientation.DiagonalA:
                zAngle = 45f;
                break;
            case GameManager.Orientation.DiagonalB:
                zAngle = 135f;
                break;
        }
        Transform spawnedObject = Instantiate(greenWinLine,
         GetGridToWorldPosition(line.centerGridVector.x, line.centerGridVector.y),
         Quaternion.Euler(0, 0, zAngle));
        spawnedObject.GetComponent<NetworkObject>().Spawn(true); //Spawns objects to clients too.
    }

    void ClickedOnGridVisual(int x, int y, GameManager.PlayerType playerType)
    {
        SpawnObjectRPC(x, y, playerType);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRPC(int x, int y, GameManager.PlayerType playerType)
    {
        Transform prefab;
        switch (playerType)
        {
            case GameManager.PlayerType.Cross:
                prefab = crossPrefeb;
                break;
            case GameManager.PlayerType.Circle:
                prefab = circlePrefab;
                break;
            default:
                prefab = crossPrefeb;
                break;
        }

        Transform spawnedObject = Instantiate(prefab, GetGridToWorldPosition(x, y), Quaternion.identity);
        spawnedObject.GetComponent<NetworkObject>().Spawn(true); //Spawns objects to clients too.
    }

    private Vector2 GetGridToWorldPosition(int x, int y)
    {
        Debug.Log(((y * GRIDSIZE) - GRIDSIZE) + " " + ((x * GRIDSIZE) - GRIDSIZE));
        return new Vector2((y * GRIDSIZE) - GRIDSIZE, (x * GRIDSIZE) - GRIDSIZE);
    }

}

