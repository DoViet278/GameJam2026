using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static CONST;

public class CollectItemInSafe : MonoBehaviour
{
    public GameObject puzzle2;
    public GameObject cardPrefab;
    public Sprite cardSprite;

    public Transform spawnPointCoin;
    public Transform spawnPointCard;

    public float flyDuration = .25f;
    private bool allowCollect;
    private ListItemSelected listItemSelected;
    
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
        allowCollect = false;
        listItemSelected = FindObjectOfType<ListItemSelected>();    
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(GameController.instance.safeOpended && collision.gameObject.name == "Player")
        {
            allowCollect = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        allowCollect = false;
    }   

    private void OnPlayerPressE(){
        if (allowCollect)
        {
            allowCollect = false;
            StartCoroutine(SpawnAndFlyItems());
        }
    }

    private IEnumerator SpawnAndFlyItems()
    {
        GameObject card = Instantiate(cardPrefab, spawnPointCard.position, Quaternion.identity);

        Vector3 startPosCard = card.transform.position;

        float t = 0;
        Transform player = GameObject.Find("Player").transform;
        Vector3 targetPos = player.position;
        while(t < flyDuration) 
        {
            t += Time.deltaTime;
            card.transform.position = Vector3.Lerp(startPosCard, targetPos, t / flyDuration);
            yield return null;
        }
        GameController.instance.hasCard = true;
        Destroy(card);
        listItemSelected.AddItem("Card", cardSprite);
        puzzle2.SetActive(true);
    }

}
