using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListItemSelected : MonoBehaviour
{
    public Transform itemBar;     
    public GameObject itemIconPrefab;

    private HashSet<string> collectedItems = new HashSet<string>();

    public void AddItem(string itemId, Sprite icon)
    {
        if (collectedItems.Contains(itemId))
            return;

        collectedItems.Add(itemId);

        GameObject iconGO = Instantiate(itemIconPrefab, itemBar);
        Image img = iconGO.GetComponent<Image>();
        img.sprite = icon;
    }
}
