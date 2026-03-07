using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CONST;

public class TriggerPuzzle : MonoBehaviour
{
    public GameObject puzzle;

    public GameObject outline;
    
    private bool played = false;
    private bool isPlayerNearby;

    private void OnEnable()
    {
        ObserverManager.Register(PLAYER_PRESS_E, (Action)OnPlayerPressE);
    }
    
    private void OnDisable()
    {
        ObserverManager.Unregister(PLAYER_PRESS_E, (Action)OnPlayerPressE);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.name == "Player")
        {
            isPlayerNearby =true;
            outline.SetActive(true);
           
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        isPlayerNearby=false;
        outline.SetActive(false);
    }

    
    private void OnPlayerPressE(){
        if (isPlayerNearby)
        {
            if(!played) QuestManager.Instance.CompleteCurrentQuest();
            played = true;
            puzzle.SetActive(true);
        }
    }
}
