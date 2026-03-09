using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CONST;

public class CollectKeyInShelf : MonoBehaviour
{
    public GameObject keyPrefab;
    public Transform spawnPoint;
    public Sprite keySprite;

    public float riseHeight = 1.2f;
    public float riseDuration = 1f;
    public float flyDuration = 0.6f;

    private bool searched;
    private bool isPlayerNearby;
    private ListItemSelected listItemSelected;
    
    //Lang nghe sk nhan phim E

    private void Awake()
    {
        listItemSelected = FindObjectOfType<ListItemSelected>();
    }

    private void OnEnable()
    {
        ObserverManager.Register(PLAYER_PRESS_E, (Action)OnPlayerPressE);
    }
    
    private void OnDisable()
    {
        ObserverManager.Unregister(PLAYER_PRESS_E, (Action)OnPlayerPressE);
    }

    private void Start()
    {
        searched = false;
        isPlayerNearby = false;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            isPlayerNearby =true;   
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isPlayerNearby=false;
    }

    private void OnPlayerPressE(){
        if (!searched && isPlayerNearby)
        {
            searched = true;
            StartCoroutine(SpawnAndFlyKey());
        }
    }

    IEnumerator SpawnAndFlyKey()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject key = Instantiate(keyPrefab, spawnPoint.position, Quaternion.identity);

        Vector3 startPos = key.transform.position;
        Vector3 risePos = startPos + Vector3.up * riseHeight;

        float t = 0;
        while (t < riseDuration)
        {
            t += Time.deltaTime;
            key.transform.position = Vector3.Lerp(startPos, risePos, t / riseDuration);
            yield return null;
        }

        Transform player = GameObject.Find("Player").transform;
        Vector3 targetPos = player.position;

        t = 0;
        while (t < flyDuration)
        {
            t += Time.deltaTime;
            key.transform.position = Vector3.Lerp(risePos, targetPos, t / flyDuration);
            yield return null;
        }

        GameController.instance.hasKeySecurityRoom = true;
        QuestManager.Instance.CompleteCurrentQuest(); 
        Destroy(key);
        listItemSelected.AddItem("KeySecurityRoom", keySprite);
    }
}
