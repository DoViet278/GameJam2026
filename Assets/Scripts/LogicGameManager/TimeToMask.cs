using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeToMask : MonoBehaviour
{
    private float timeToMask = 45f;
    private float currTime;
    private float timeRunMask = 1f;
    private Slider timeSlider;
    public bool isMasking = false;
    private void Start()
    {
        currTime = timeToMask;
        UpdateSlider();
    }
    public void StartToMask()
    {
        ResetTime();
        if(!isMasking)
        {
            StartCoroutine(StartMask());
        }
    }

    public void ResetTime()
    {
        currTime = timeToMask;
    }

    private IEnumerator StartMask()
    {
        isMasking = true;
       while(currTime > 0)
       {
            currTime -= timeRunMask;
            if (currTime < 0) break;
            UpdateSlider();
            yield return new WaitForSeconds(timeRunMask);
       }
       isMasking = false;
       GameController.instance.index = 0;
    }

    public void UpdateSlider()
    {
        timeSlider = GameObject.Find("TimeSlider").GetComponent<Slider>();
        if (timeSlider != null)
        {
            timeSlider.value = currTime;
            timeSlider.maxValue = timeToMask;
        }
    }
}
