using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;
    [SerializeField] private GameObject passwordInfo;
    [SerializeField] private TextMeshProUGUI txtNoti;
    [SerializeField] private TextMeshProUGUI txtWin;
    [SerializeField] private Button btnExit;
    public float[] puzzlePosition;
    public PuzzlePiece[] PuzzlePieces;
    
    public int draggingPiece = -1;
    public Canvas canvas;
    
    public SafeDialController safeDialController;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitPiece();
        Shuffle();
    }

    private void OnEnable()
    {
        btnExit.onClick.AddListener(ExitPuzzle);
        PlayerController.instance.DisableInput();
        passwordInfo.SetActive(false);
        txtNoti.text = "Sắp xếp lại ảnh!";
        txtWin.text = "";   
    }

    private void InitPiece()
    {
        float startPos = PuzzlePieces[0].transform.localPosition.y;
        float dist = PuzzlePieces[0].rect.sizeDelta.y;
        for(int i = 1; i < PuzzlePieces.Length; i++)
        {
            PuzzlePieces[i].transform.localPosition = new Vector3(0, startPos - dist*i, 0);
        }
        
        puzzlePosition = new float[PuzzlePieces.Length];
        for (int i = 0; i < PuzzlePieces.Length; i++)
        {
            PuzzlePieces[i].pieceIndex = i;
            PuzzlePieces[i].currentPieceIndex = i;
            puzzlePosition[i] = PuzzlePieces[i].transform.localPosition.y;
        }
    }

    public int GetPieceIndex(PuzzlePiece piece)
    {
        if(piece.transform.localPosition.y > puzzlePosition[piece.currentPieceIndex] + piece.rect.sizeDelta.y/2)
        {
            if (piece.currentPieceIndex == 0)
                return piece.currentPieceIndex;
            return piece.currentPieceIndex - 1;
        }
        
        if(piece.transform.localPosition.y < puzzlePosition[piece.currentPieceIndex] - piece.rect.sizeDelta.y/2)
        {
            if(piece.currentPieceIndex == PuzzlePieces.Length - 1) 
                return piece.currentPieceIndex;
            return piece.currentPieceIndex + 1;
        }
        
        return piece.currentPieceIndex;
    }

    public float GetPiecePosition(int pieceIndex)
    {
        return puzzlePosition[pieceIndex];
    }

    public void ChangePiecePosition(int pieceIndex, int targetIndex)
    {
        float targetY = puzzlePosition[pieceIndex];
        Vector3 targetPosition = new Vector3(0, targetY, 0);
        PuzzlePieces[targetIndex].transform.localPosition = targetPosition;
        PuzzlePiece pieceA = PuzzlePieces[pieceIndex];
        PuzzlePiece pieceB = PuzzlePieces[targetIndex];
        pieceA.currentPieceIndex = targetIndex;
        pieceB.currentPieceIndex = pieceIndex;
        PuzzlePieces[pieceIndex] = pieceB;
        PuzzlePieces[targetIndex] = pieceA;
    }
    

    public void CheckWin()
    {
        foreach (var piece in PuzzlePieces)
        {
            if (piece.currentPieceIndex != piece.pieceIndex)
                return;
        }

        WinPuzzle();
        
    }

    private void ShowNumber()
    {
        List<int> nums = new List<int>();
        int a = Random.Range(0, 101);
        nums.Add(a);
        int b;
        do { b = Random.Range(0, 101); }
        while (b == a);
        nums.Add(b);
        int c;
        do { c = Random.Range(0, 101); }
        while (c == a || c == b);
        nums.Add(c);
        txtNoti.text = "Mật khẩu két sắt";
        passwordInfo.SetActive(true);
        txtWin.text = $"{a} - {b} - {c}";
        safeDialController.correctCode = nums;
        QuestManager.Instance.CompleteCurrentQuest();
        foreach (var num in nums)
        {
            Debug.Log(num.ToString());
        }
    }

    private void WinPuzzle()
    {
        PlayerController.instance.EnableInput();
        Debug.Log("Puzzle win!");
        ShowNumber();
    }

    private void Shuffle()
    {
        System.Random random = new System.Random();

        for (int i = PuzzlePieces.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            PuzzlePieces[i].rect.localPosition = new Vector3(0,puzzlePosition[j],0);
            PuzzlePieces[i].currentPieceIndex = j;
            PuzzlePieces[j].rect.localPosition = new Vector3(0,puzzlePosition[i],0);
            PuzzlePieces[j].currentPieceIndex = i;
            (PuzzlePieces[i], PuzzlePieces[j]) = (PuzzlePieces[j], PuzzlePieces[i]);
        }
    }

    public void ExitPuzzle() 
    {
        this.gameObject.SetActive(false);
        PlayerController.instance.EnableInput();
    }
}
