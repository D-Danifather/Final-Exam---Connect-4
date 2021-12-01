using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Mask Mask;

    [SerializeField] private GameObject redPrefab = null;
    [SerializeField] private GameObject yellowPrefab = null;

    // Ghost prefabs will show, which player has the turn
    [SerializeField] private GameObject redGhostPrefab = null;
    [SerializeField] private GameObject yellowGhostPrefab = null;

    // This will be around coins, if any mask wins at each direction
    [SerializeField] private GameObject winningCircle = null;

    // Spawnpoint for player tunr --> at the top of boarder
    [SerializeField] private GameObject[] spawnPoints;

    private GameObject whichMaskIsfalling = null;

    private readonly int heightOfBoard = 6;
    private readonly int lengthOfBoard = 7;

    private int[,] boardMaskState; // 0 = empty | 1 = red | 2 = yellow
    // 0 0 0 0 0 0 0 
    // 0 0 0 0 0 0 0 
    // 0 0 0 0 0 0 0 
    // 0 0 0 0 0 0 0 
    // 0 0 0 0 0 0 0 
    // 0 0 0 0 0 0 0 

    private SoundManager SoundManager;

    private bool canPlay = true;

    private void Start()
    {
        boardMaskState = new int[lengthOfBoard, heightOfBoard];
        Mask = Mask.Red;
        redGhostPrefab.SetActive(false);
        yellowGhostPrefab.SetActive(false);
        SoundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    public void GetSelectedColumn(int column)
    {
        CheckAndTakeTurn(column);
    }

    private void CheckAndTakeTurn(int column)
    {
        redGhostPrefab.SetActive(false);
        yellowGhostPrefab.SetActive(false);

        // Checks if the column is full and if the coin is steal moving --> so it won't show any ghost over the column or take turn for the next player
        // At the biggining is the falling coin always null!
        // Blocking other players turn, while velosity != 0 --> also while its steal falling
        if (boardMaskState[column, heightOfBoard - 1] == 0 && (whichMaskIsfalling == null || whichMaskIsfalling.GetComponent<Rigidbody2D>().velocity == Vector2.zero) && canPlay)
        {
            // Checks the board state for each players index --> 0 = Empty | 1 = Red | 2 = Yellow
            if (UpdateBoardState(column))
            {
                switch (Mask)
                {
                    case Mask.Red:
                        whichMaskIsfalling = Instantiate(redPrefab, spawnPoints[column].transform.position, Quaternion.identity);
                        if (DidWin(1))
                        {
                            SoundManager.PlaySound();
                            canPlay = false;
                        }
                        SoundManager.PlaySound();
                        Mask = Mask.Yellow;
                        break;
                    case Mask.Yellow:
                        whichMaskIsfalling = Instantiate(yellowPrefab, spawnPoints[column].transform.position, Quaternion.identity);
                        if (DidWin(2))
                        {
                            SoundManager.PlaySound();
                            canPlay = false;
                        }
                        SoundManager.PlaySound();
                        Mask = Mask.Red;
                        break;
                    default:
                        break;
                }
            }

            if (DidDraw())
            {
                SoundManager.PlaySound();
                canPlay = false;
            }
        }      
    }

    public void ShowMaskGhostOverColumn(int column)
    {
        switch (Mask)
        {
            case Mask.Red:
                redGhostPrefab.SetActive(true);
                redGhostPrefab.transform.position = spawnPoints[column].transform.position;
                break;
            case Mask.Yellow:
                yellowGhostPrefab.SetActive(true);
                yellowGhostPrefab.transform.position = spawnPoints[column].transform.position;
                break;
            default:
                break;
        }
    }

    private bool UpdateBoardState(int column)
    {
        for (int row = 0; row < heightOfBoard; row++)
        {
            if (boardMaskState[column, row] == 0) // Found empty spot
            {
                // Empty = 0 | Red = 1 | Yellow = 2
                int check = Mask == Mask.Red ? boardMaskState[column, row] = 1 : boardMaskState[column, row] = 2;

                //Debug.Log($"Piece spawned at: {column}, {row}, and check is: {check}");

                return true;
            }
        }

        return false;
    }

    public bool DidDraw()
    {
        // Check, if the board is full --> if yes and nobody has won, its a draw
        for (int x = 0; x < lengthOfBoard; x++)
            if (boardMaskState[x, heightOfBoard - 1] == 0)
                return false;

        return true;
    }

    public bool DidWin(int maskNumber)
    {
        // Horizontal
        for (int x = 0; x < lengthOfBoard - 3; x++)
            for (int y = 0; y < heightOfBoard; y++)
                if (boardMaskState[x, y] == maskNumber && boardMaskState[x + 1, y] == maskNumber && boardMaskState[x + 2, y] == maskNumber && boardMaskState[x + 3, y] == maskNumber)
                    return true;

        // 0 0 0 0 0 0 0 
        // 0 0 0 0 0 0 0 
        // 0 0 0 0 0 0 0 
        // 0 0 0 0 0 0 0 
        // 0 0 0 0 0 0 0 
        // 1 1 1 1 0 0 0 

        // Vertical
        for (int x = 0; x < lengthOfBoard; x++)
            for (int y = 0; y < heightOfBoard - 3; y++)
                if (boardMaskState[x, y] == maskNumber && boardMaskState[x, y + 1] == maskNumber && boardMaskState[x, y + 2] == maskNumber && boardMaskState[x, y + 3] == maskNumber)
                    return true;

        // 0 0 0 0 0 0 0 
        // 0 0 0 0 0 0 0 
        // 1 0 0 0 0 0 0 
        // 1 0 0 0 0 0 0 
        // 1 0 0 0 0 0 0 
        // 1 0 0 0 0 0 0 

        // Diagonal (y + x line) --> combination of both lines --> diagonal has got two different win variants
        for (int x = 0; x < lengthOfBoard - 3; x++)
            for (int y = 0; y < heightOfBoard - 3; y++)
                if (boardMaskState[x, y] == maskNumber && boardMaskState[x + 1, y + 1] == maskNumber && boardMaskState[x + 2, y + 2] == maskNumber && boardMaskState[x + 3, y + 3] == maskNumber)
                    return true;

        // 0 0 0 0 0 0 0 
        // 0 0 0 0 0 0 0 
        // 0 0 0 1 0 0 0 
        // 0 0 1 0 0 0 0 
        // 0 1 0 0 0 0 0 
        // 1 0 0 0 0 0 0 

        for (int x = 0; x < lengthOfBoard - 3; x++)
            for (int y = 0; y < heightOfBoard - 3; y++)
                if (boardMaskState[x, y + 3] == maskNumber && boardMaskState[x + 1, y + 2] == maskNumber && boardMaskState[x + 2, y + 1] == maskNumber && boardMaskState[x + 3, y] == maskNumber)
                    return true;
        
        // 0 0 0 0 0 0 0 
        // 0 0 0 0 0 0 0 
        // 1 0 0 0 0 0 0 
        // 0 1 0 0 0 0 0 
        // 0 0 1 0 0 0 0 
        // 0 0 0 1 0 0 0 

        return false;
    }
}
