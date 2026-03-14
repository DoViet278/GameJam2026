using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerNear : MonoBehaviour
{
    public Transform player;
    public float detectDistance = 4f;

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer < detectDistance)
        {
            ActiveChild();
        }
        else
        {
            DeactiveChild();
        }
    }

    private void ActiveChild()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void DeactiveChild()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
}
