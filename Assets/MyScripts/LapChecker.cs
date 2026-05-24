using System;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Vehicles.Car;

public class LapChecker : MonoBehaviour
{
    [SerializeField] private int totalLaps = 0;
    [SerializeField] private Checkpoint[] checkpoints;


    [SerializeField] private TMP_Text lapInfo;

    //Referencias
    [SerializeField] private TimeController timer;
    [SerializeField] private AutoCam cam;
    private int playerLaps = 0;

    //Events
    public static event Action<int> OnLapCompleted;
    public static event Action<Transform, float, bool> OnRaceCompleted;
    private void Awake()
    {
        ResetCheckpoints();
        UpdateUI();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && AllCheckpointsValidated())
        {
            AddLap();

            if (playerLaps >= totalLaps)
            {
                ResetPlayer(other);

                EndGame(other.transform);
            }
            else UpdateUI();

            ResetCheckpoints();
        }

        if (other.CompareTag("Enemy"))
        {
            CarAIControl aiLap = other.GetComponent<CarAIControl>();
            aiLap.laps++;

            if (aiLap.laps > totalLaps)
            {
                aiLap.enabled = false;
                other.enabled = false;
                EndGame(other.transform);
            }
        }
    }

    private bool AllCheckpointsValidated()
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (!checkpoints[i].validated) return false;
        }
        return true;
    }

    private void UpdateUI()
    {
        lapInfo.text = "Lap: " + playerLaps + " / " + totalLaps;
    }

    private void AddLap()
    {
        playerLaps++;
        OnLapCompleted?.Invoke(playerLaps);
    }

    private void EndGame(Transform other)
    {
        OnRaceCompleted?.Invoke(other, timer.GetTotalRaceTime(), timer.IsBestRace());
    }

    private void ResetCheckpoints()
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].validated = false;
        }
    }

    private void ResetPlayer(Collider other)
    {
        other.GetComponent<CarUserControl>().enabled = false;
        other.GetComponent<CarAudio>().StopSound();
        other.GetComponent<StuckDetector>().enabled = false;
        other.enabled = false;
    }
}
