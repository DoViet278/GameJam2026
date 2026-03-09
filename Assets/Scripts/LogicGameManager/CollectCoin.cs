using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CONST;

public class CollectCoin : MonoBehaviour
{
    private bool isPlayerNearby;
    private void OnEnable()
    {
        ObserverManager.Register(PLAYER_PRESS_E, (Action)OnPlayerPressE);
    }

    private void OnDisable()
    {
        ObserverManager.Unregister(PLAYER_PRESS_E, (Action)OnPlayerPressE);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            isPlayerNearby = true;
            UIConntroller.instance.ShowTutorialKey();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            isPlayerNearby = false;
            UIConntroller.instance.HideTutorialKey();
        }
    }

    private void OnPlayerPressE()
    {
        if (isPlayerNearby)
        {
            GameController.instance.hasCoin = true;
            QuestManager.Instance.CompleteCurrentQuest();
            Destroy(gameObject);
        }
    }
}
