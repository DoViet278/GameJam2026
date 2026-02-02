using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;
    private BoxCollider2D boxCollider;
    private bool isOpen = false;
    private bool canOpen = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerController = FindObjectOfType<PlayerController>();
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            canOpen = true;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && canOpen)
        {
            StartCoroutine(OpenDoor());
        }
    }

    private IEnumerator OpenDoor()
    {
        canOpen = false;
        isOpen = true;
        animator.SetBool("open",true);
        boxCollider.enabled = false;
        yield return null;
        isOpen = false;

    }

}
