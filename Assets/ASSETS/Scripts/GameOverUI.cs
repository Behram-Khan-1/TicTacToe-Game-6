using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Hide();
        GameManager.Instance.OnGameWin += GameManager_OnWin;
    }

    public void GameManager_OnWin(GameManager.Line line, GameManager.PlayerType winPlayerType)
    {
        if (winPlayerType == GameManager.Instance.GetPlayerType())
        {
            gameOverText.text = "You Win!";
            gameOverText.color = winColor;
        }
        else
        {
            gameOverText.text = "You Lose!";
            gameOverText.color = loseColor;
        }
        Show();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }


}
