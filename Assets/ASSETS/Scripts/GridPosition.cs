using UnityEngine;

public class GridPosition : MonoBehaviour
{
    [SerializeField] private int x;
    [SerializeField] private int y;
    void OnMouseDown()
    {
        GameManager.Instance.ClickedOnGridPositionRPC(x, y, GameManager.Instance.GetPlayerType());
    }
}
