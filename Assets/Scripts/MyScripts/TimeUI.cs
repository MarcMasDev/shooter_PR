using TMPro;
using UnityEngine;
using System;

public class TimeUI : MonoBehaviour
{
    private TMP_Text timerText;

    void Start()
    {
        timerText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        timerText.text = FormatTime(Time.timeSinceLevelLoad);
    }

    private string FormatTime(float timeInSeconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(timeInSeconds);
        return string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
    }
}
