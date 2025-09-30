using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject circleArrow;
    [SerializeField] private GameObject crossArrow;
    [SerializeField] private GameObject circleYouText;
    [SerializeField] private GameObject crossYouText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        circleArrow.SetActive(false);
        crossArrow.SetActive(false);
        circleYouText.SetActive(false);
        crossYouText.SetActive(false);
    }
    void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.OnTurnChange += UpdateCurrentArrow;
    }

    void GameManager_OnGameStarted()
    {
        if (GameManager.Instance.GetPlayerType() == GameManager.PlayerType.Circle)
        {
            circleYouText.SetActive(true);
        }
        else
            crossYouText.SetActive(true);

        UpdateCurrentArrow();
    }

    void UpdateCurrentArrow()
    {
        if (GameManager.Instance.GetCurrentPlayer() == GameManager.PlayerType.Circle)
        {
            circleArrow.SetActive(true);
            crossArrow.SetActive(false);
        }
        else
        {
            circleArrow.SetActive(false);
            crossArrow.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
