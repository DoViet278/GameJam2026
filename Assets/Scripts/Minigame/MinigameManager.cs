using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    public Piece[] pieces;
    public static int size = 3;
    public static MinigameManager instance;
    public bool Clickable = true;
    private List<Vector2> positions = new List<Vector2>();
    
    public Button exitButton;
    public GameObject puzzle;
    public GameObject passwordInfo;
    public TextMeshProUGUI txtNoti;
    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        exitButton.onClick.AddListener(ExitPuzzle);
        PlayerController.instance.DisableInput();
        
        passwordInfo.SetActive(false);
        txtNoti.text = "Try to solve the puzzle!";
    }

    private void Start()
    {
        PrepareMinigame();
    }

    public void PrepareMinigame()
    {
        if (pieces.Length + 1 < size * size)
        {
            Debug.LogWarning("Not enough piece");
        }
        
        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].Init(i);
            positions.Add(pieces[i].rect.localPosition);
        }

        Shuffle();
    }

    public void ChangeTile(Piece movePiece)
    {
        int index = Array.IndexOf(pieces, movePiece);

        Vector2Int piecePos = GetPosition(index);
        int targetIndex = CheckBlankTile(piecePos.x, piecePos.y);
        if (targetIndex != -1)
        {
            StartCoroutine(SwapPieces(index, targetIndex));
        }
    }

    IEnumerator SwapPieces(int index, int targetIndex)
    {
        Clickable = false;
        Vector2 start = pieces[index].rect.localPosition;
        float duration = 0.1f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            pieces[index].rect.localPosition = Vector3.Lerp(start, positions[targetIndex], t);
            
            yield return null;
        }
        pieces[index].rect.localPosition = positions[targetIndex];
        pieces[targetIndex].rect.localPosition = start;
        
        yield return new WaitForSeconds(duration);
        Clickable = true;
        (pieces[targetIndex], pieces[index]) = (pieces[index], pieces[targetIndex]);
        CheckWin();
    }

    private int CheckBlankTile(int x, int y)
    {
        if (y + 1 < size)
        {
            if (pieces[GetIndex(x, y + 1)].isBlank)
                return GetIndex(x, y + 1);
        }

        if (y - 1 >= 0)
        {
            if (pieces[GetIndex(x, y - 1)].isBlank)
                return GetIndex(x, y - 1);
        }
        
        if(x + 1 < size)
        {
            if (pieces[GetIndex(x+1,y)].isBlank)
                return GetIndex(x+1,y);
        }
        
        if(x - 1 >= 0)
        {
            if (pieces[GetIndex(x-1,y)].isBlank)
                return GetIndex(x-1,y);
        }
        
        return -1;
    }

    private void CheckWin()
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i]._id != i)
            {
                return;
            }
        }
        WinPuzzle();
    }

    private void WinPuzzle()
    {
        puzzle.SetActive(false);
        passwordInfo.SetActive(true);
        txtNoti.text = "Mật khẩu kho vàng";
        
    }

    private void ExitPuzzle()
    {
        gameObject.SetActive(false);
        PlayerController.instance.EnableInput();
    }

    private void Shuffle()
    {
        System.Random random = new System.Random();

        for (int i = pieces.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            pieces[i].rect.localPosition = positions[j];
            pieces[j].rect.localPosition = positions[i];
            (pieces[j], pieces[i]) = (pieces[i], pieces[j]);
        }
        
        if (!IsSolvable())
        {
            pieces[0].rect.localPosition = positions[1];
            pieces[1].rect.localPosition = positions[0];
            (pieces[0], pieces[1]) = (pieces[1], pieces[0]);
        }
        
    }
    
    bool IsSolvable()
    {
        int inversion = 0;

        for (int i = 0; i < pieces.Length; i++)
        {
            for (int j = i + 1; j < pieces.Length; j++)
            {
                if (pieces[i]._id == 0 || pieces[j]._id == 0)
                    continue;

                if (pieces[i]._id > pieces[j]._id)
                    inversion++;
            }
        }

        return inversion % 2 == 0;
    }
    
    Vector2Int GetPosition(int index)
    {
        int x = index % size;
        int y = index / size;
        return new Vector2Int(x, y);
    }

    int GetIndex(int x, int y)
    {
        return y * size + x;
    }
}
