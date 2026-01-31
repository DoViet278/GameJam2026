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
    public bool endMask = false;
    private void Start()
    {
        currTime = timeToMask;
        UpdateSlider();
    }
    public void StartToMask()
    {
        endMask = false;
        StartCoroutine(StartMask());
    }

    private IEnumerator StartMask()
    {
       while(currTime > 0)
       {
            currTime -= timeRunMask;
            if (currTime < 0) break;
            UpdateSlider();
            yield return new WaitForSeconds(timeRunMask);
       }
       endMask = true;
       GameController.instance.index = 0;
    }

    public void UpdateSlider()
    {
        timeSlider = GameObject.Find("TimeSlider").GetComponent<Slider>();
        timeSlider.value = currTime;
        timeSlider.maxValue = timeToMask;
    }
}
