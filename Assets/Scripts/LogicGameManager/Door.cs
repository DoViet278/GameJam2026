using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CONST;

public class Door : MonoBehaviour
{
    private enum DoorType
    {
        NoKey,
        KeyRequired,
        CardRequired    
    }

    [SerializeField] private DoorType type;
    private GameObject lockDoor;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private bool canOpen = false;
    private PlayerActionSfx doorSfx;
    
    private void OnEnable()
    {
        ObserverManager.Register(PLAYER_PRESS_E, (Action)OnPlayerPressE);
    }
    
    private void OnDisable()
    {
        ObserverManager.Unregister(PLAYER_PRESS_E, (Action)OnPlayerPressE);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        if(type == DoorType.KeyRequired)
        {
            lockDoor = transform.GetChild(0).gameObject;
        }
        doorSfx = GetComponent<PlayerActionSfx>();
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            if(type == DoorType.NoKey) canOpen = true;
            else if(type == DoorType.KeyRequired && GameController.instance.hasKeySecurityRoom)
            {
                canOpen = true;
                lockDoor.SetActive(false);
            }
            else if(type == DoorType.CardRequired && GameController.instance.hasCard) canOpen = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        canOpen = false;
    }

    private void OnPlayerPressE()
    {
        if(canOpen)
        {
            StartCoroutine(OpenDoor());
        }
    }

    private IEnumerator OpenDoor()
    {
        canOpen = false;
        doorSfx.PlayAction("Open");
        animator.SetBool("open",true);
        boxCollider.enabled = false;
        yield return null;
    }

}

