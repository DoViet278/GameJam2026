using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectKeyInShelf : MonoBehaviour
{
    public GameObject keyPrefab;
    public Transform spawnPoint;
    public float riseHeight = 1.2f;
    public float riseDuration = 1f;
    public float flyDuration = 0.6f;

    private bool searched;
    private bool isPlayerNearby;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !searched && isPlayerNearby)
        {
            searched = true;
            StartCoroutine(SpawnAndFlyKey());
        }
    }

    IEnumerator SpawnAndFlyKey()
    {
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
        // inventory.AddKey();
    }
}
