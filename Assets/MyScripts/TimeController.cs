using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    private float lapTime = 0;
    private List<float> lapsTimes = new List<float>();
    private List<float> lapsGhostDifferences = new List<float>();

    [SerializeField] private TMP_Text totalTimeText;
    [SerializeField] private TMP_Text totalLapsText;

    public float bestRaceTime = Mathf.Infinity;

    [Header("Pop-up")]
    [SerializeField] private TMP_Text currentTimePopUpText;
    [SerializeField] private TMP_Text allTimePopUpText;
    [SerializeField] private TMP_Text ghostDifferencePopUpText;
    [SerializeField] private CanvasGroup popUp;

    [Header("Ghost")]
    [SerializeField] private GhostLapData bestGhostData;
    [SerializeField] private CanvasGroup ghostDiffPopup;
    private int currentCheckpoint = 0;

    //Listener
    private void OnEnable()
    {
        Checkpoint.OnCheckpointReached += HandleCheckpoint;
        LapChecker.OnLapCompleted += NewLap;
    }

    private void OnDisable()
    {
        Checkpoint.OnCheckpointReached -= HandleCheckpoint;
        LapChecker.OnLapCompleted -= NewLap;
    }

    private void Start()
    {
        if (bestGhostData == null || bestGhostData.checkpointTimes.Count == 0) ghostDiffPopup.alpha = 0;
        else ghostDiffPopup.alpha = 1;
    }
    private void Update()
    {
        lapTime += Time.deltaTime;
        totalTimeText.text = FormatTime(Time.timeSinceLevelLoad);
        UpdateLapUI();
    }

    void HandleCheckpoint(Checkpoint cp)
    {
        currentCheckpoint++;
        StartCoroutine(SendTimeMessage(cp));
    }

    void NewLap(int lap)
    {
        float finishedTime = lapTime;
        currentCheckpoint++;

        float lapDelta = GhostDifferenceCheckPoint();
        lapsGhostDifferences.Add(lapDelta);

        lapsTimes.Add(finishedTime);
        lapTime = 0;

        StartCoroutine(SendTimeMessage(null, finishedTime));
    }

    private IEnumerator SendTimeMessage(Checkpoint cp = null, float lapDisplayTime = 0)
    {
        float timeToUse = (cp == null) ? lapDisplayTime : lapTime;

        currentTimePopUpText.text = FormatTime(timeToUse);

        bool bestTime = true;
        allTimePopUpText.text = "";

        if (cp != null)
        {
            for (int i = 0; i < cp.lapsTimes.Count; i++)
            {
                if (cp.lapsTimes[i] < timeToUse) bestTime = false;
                allTimePopUpText.text += "Lap " + (i + 1) + ": " + FormatTime(cp.lapsTimes[i]) + "\n";
            }
            cp.lapsTimes.Add(timeToUse);
        }
        else
        {
            for (int i = 0; i < lapsTimes.Count; i++)
            {
                if (i < lapsTimes.Count - 1 && lapsTimes[i] < timeToUse)
                {
                    bestTime = false;
                }
                allTimePopUpText.text += "Lap " + (i + 1) + ": " + FormatTime(lapsTimes[i]) + "\n";
            }
        }

        currentTimePopUpText.color = bestTime ? Color.green : Color.white;
        popUp.alpha = 1;

        CheckpointGhostTime();

        yield return new WaitForSeconds(2f);

        popUp.alpha = 0;
    }

    private void UpdateLapUI()
    {
        string text = "";

        for (int i = 0; i < lapsTimes.Count; i++)
        {
            float diff = lapsGhostDifferences[i];

            text += "Lap " + (i + 1) + ": "
                + FormatTime(lapsTimes[i])
                + " | "
                + FormatGhostText(diff)
                + FormatTime(Mathf.Abs(diff))
                + "</color>\n";
        }

        text += "Lap " + (lapsTimes.Count + 1) + ": " + FormatTime(lapTime);
        totalLapsText.text = text;
    }
    private void CheckpointGhostTime()
    {
        ghostDifferencePopUpText.text = GetGhostCurrentTimeText();
    }

    private string GetGhostCurrentTimeText()
    {
        if (bestGhostData.checkpointTimes.Count <= 0) return "";
        float diff = GhostDifferenceCheckPoint();

        return FormatGhostText(diff) + FormatTime(Mathf.Abs(diff)) + "</color>";
    }

    public float GetTotalRaceTime()
    {
        float total = 0;

        for (int i = 0; i < lapsTimes.Count; i++)
            total += lapsTimes[i];

        total += lapTime;

        return total;
    }

    public bool IsBestRace()
    {
        return GetTotalRaceTime() < bestGhostData.raceTime;
    }

    private string FormatGhostText(float ghostTimeDifference)
    {
        if (ghostTimeDifference < 0)
            return "<color=green>-";
        else if (ghostTimeDifference > 0)
            return "<color=red>+";
        else
            return "<color=white>";
    }
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        float seconds = time % 60f;
        if (minutes <= 0) return seconds.ToString("00.00");
        return minutes.ToString("00") + ":" + seconds.ToString("00.00");
    }
    private float GhostDifferenceCheckPoint()
    {
        if (bestGhostData.checkpointTimes.Count == 0) return 0;

        int index = currentCheckpoint - 1;
        if (index < 0 || index >= bestGhostData.checkpointTimes.Count) return 0;

        return GetTotalRaceTime() - bestGhostData.checkpointTimes[index];
    }

}
