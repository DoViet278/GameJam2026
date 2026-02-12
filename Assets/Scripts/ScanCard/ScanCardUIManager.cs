using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanCardUIManager : MonoBehaviour
{
    [SerializeField] private GameObject imgCard;

    private void OnEnable()
    {
        if(GameController.instance.hasCard)
        {
            imgCard.SetActive(true);
        }
        else
        {
            imgCard.SetActive(false);
        }
    }
    
}
