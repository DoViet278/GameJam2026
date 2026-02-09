using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHasCoin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            if(GameController.instance.hasCoin)
            {
                GameController.instance.isWin = true;
            }
        }
    }
}
