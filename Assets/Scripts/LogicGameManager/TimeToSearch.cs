using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToSearch : MonoBehaviour
{
    private float timeToSearch = 3f;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void StartSearch()
    {
        if (playerController.isSearching)
        {
            StartCoroutine(searchCoroutine());
        }
    }

    private IEnumerator searchCoroutine() 
    {
        yield return new WaitForSeconds(timeToSearch);
        playerController.isSearching = false;
    }
}
