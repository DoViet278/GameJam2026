using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayOutro : MonoBehaviour
{

    private void OnEnable()
    {
        UIConntroller.instance.PlayOutro();
    }
}
