using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player" && Input.GetKeyDown(KeyCode.E))
        {
            GameController.instance.hasCoin = true; 
            Destroy(gameObject);
        }
    }
}
