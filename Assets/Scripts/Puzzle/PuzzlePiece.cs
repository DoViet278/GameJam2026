using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform rect;
    public int pieceIndex;
    public int currentPieceIndex;
    
    
    private int endIndex;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(PuzzleManager.instance.draggingPiece == -1)
            PuzzleManager.instance.draggingPiece = pieceIndex;
        endIndex = currentPieceIndex;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (PuzzleManager.instance.draggingPiece != this.pieceIndex)
            return;
        Canvas canvas = PuzzleManager.instance.canvas;
        float dy = eventData.delta.y / canvas.scaleFactor;
        rect.anchoredPosition += new Vector2(0f, dy);
        endIndex = PuzzleManager.instance.GetPieceIndex(this);
        // Debug.Log(endIndex);
        if (endIndex != currentPieceIndex)
        {
            PuzzleManager.instance.ChangePiecePosition(currentPieceIndex, endIndex);
            // currentPieceIndex = endIndex;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float pos = PuzzleManager.instance.GetPiecePosition(endIndex);
        rect.localPosition = new Vector3(0, pos, 0);

        PuzzleManager.instance.draggingPiece = -1;
        PuzzleManager.instance.CheckWin();
    }

    
    
}
