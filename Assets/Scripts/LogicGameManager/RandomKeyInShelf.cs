using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomKeyInShelf : MonoBehaviour
{
    public List<GameObject> shelfList;

    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private Sprite keySprite;

    void Start()
    {
        int randomIndex = Random.Range(0, shelfList.Count);

        GameObject shelf = shelfList[randomIndex];  
        CollectKeyInShelf script = shelf.AddComponent<CollectKeyInShelf>();

        script.keyPrefab = keyPrefab;
        script.keySprite = keySprite;
    }
}
