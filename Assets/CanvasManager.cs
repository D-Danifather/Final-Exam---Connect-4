using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Text winner = null;
    [SerializeField] private GameObject resetButtonGO = null;

    private GameManager GameManager;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        resetButtonGO.SetActive(false);
    }

    public void GetWinnerText()
    {
        if (GameManager.DidWin(1))
        {
            winner.text = "Red won!";
            resetButtonGO.SetActive(true);
        }
        else if (GameManager.DidWin(2))
        {
            winner.text = "Yellow won!";
            resetButtonGO.SetActive(true);
        }
        else if (GameManager.DidDraw())
        {
            winner.text = "Its a Draw";
            resetButtonGO.SetActive(true);
        }
        else
            winner.text = string.Empty;
    }

    public void RestartGame()
        => Application.LoadLevel(Application.loadedLevel);
}
