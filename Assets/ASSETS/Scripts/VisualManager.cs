using Unity.Netcode;
using UnityEngine;

public class VisualManager : NetworkBehaviour
{
    [SerializeField] private Transform crossPrefeb;
    [SerializeField] private Transform circlePrefab;
    private const int GRIDSIZE = 3;
    void Start()
    {
        GameManager.Instance.ClickedOnGrid += ClickedOnGridVisual;
    }

    // Update is called once per frames
    void Update()
    {

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
        Transform spawnedObject = Instantiate(prefab);
        Debug.Log("About to call GridToWorld");
        spawnedObject.position = GetGridToWorldPosition(x, y);
        spawnedObject.GetComponent<NetworkObject>().Spawn(true); //Spawns objects to clients too.
    }

    Vector2 GetGridToWorldPosition(int x, int y)
    {
        Debug.Log(x * GRIDSIZE + " " +  y * GRIDSIZE);
        return new Vector2(  (y * GRIDSIZE ) - GRIDSIZE, (x * GRIDSIZE) - GRIDSIZE);
    }
   
}

