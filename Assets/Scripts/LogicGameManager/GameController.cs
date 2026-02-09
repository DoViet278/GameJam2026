using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public int index;
    public bool isGameOver;
    public bool isWin;
    public bool hasCoin;
    public bool hasKeySecurityRoom;
    public bool hasCard;
    public bool safeOpended;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        isGameOver = false;
        isWin = false;
        hasCoin = false;
        hasKeySecurityRoom = false;
        hasCard = false;
        index = 0;
    }
}
