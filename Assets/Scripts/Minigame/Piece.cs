using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    public int _id;
    public RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        GetComponent<Button>().onClick.AddListener(OnClickPiece);
    }

    public void Init(int id)
    {
        _id = id;
    }

    private void OnClickPiece()
    {
        MinigameManager.instance.PickPiece(this);
    }
}
