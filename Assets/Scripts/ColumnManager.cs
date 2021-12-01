using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnManager : MonoBehaviour
{
    [SerializeField] private int columnsCounterFromLeftToRight;
    
    private GameManager GameManager;

    private CanvasManager CanvasManager;

    private void Awake()
    {
        columnsCounterFromLeftToRight = transform.GetSiblingIndex(); // Gets the siblings transforms child in the same parents while game starts

        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        CanvasManager = GameObject.Find("CanvasManager").GetComponent<CanvasManager>();
    }

    private void OnMouseDown()
    {
        GameManager.GetSelectedColumn(columnsCounterFromLeftToRight);
        CanvasManager.GetWinnerText();
    }

    private void OnMouseOver()
    {
        GameManager.ShowMaskGhostOverColumn(columnsCounterFromLeftToRight);
    }
}
