using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPuzzle : MonoBehaviour
{
    public GameObject puzzle;

    public GameObject outline;
    
    private bool played = false;
    private bool isPlayerNearby;

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

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !played && isPlayerNearby)
        {
            played = true;
            puzzle.SetActive(true);
        }
    }
}
