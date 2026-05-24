using System;
using TMPro;
using UnityEngine;

public class EndRace : MonoBehaviour
{
    private CanvasGroup showMenu;
    [SerializeField] private CanvasGroup[] hideMenu;
    [SerializeField] private TMP_Text textResults;

    private void Awake()
    {
        showMenu = GetComponent<CanvasGroup>();
        textResults.text = String.Empty;
    }
    private void OnEnable()
    {
        LapChecker.OnRaceCompleted += RaceEnd;
    }

    private void OnDisable()
    {
        LapChecker.OnRaceCompleted -= RaceEnd;
    }

    private void RaceEnd(Transform ender, float totalTime, bool bestRace)
    {
        if (ender.CompareTag("Player")) ShowEndMenu();

        textResults.text = textResults.text + "\n" + ender.tag.ToString() + ": " + FormatTime(totalTime);
    }
    private void ShowEndMenu()
    {
        for (int i = 0; i < hideMenu.Length; i++)
        {
            hideMenu[i].alpha = 0;
        }
        showMenu.alpha = 1;
        showMenu.interactable = true;
        showMenu.blocksRaycasts = true;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        float seconds = time % 60f;
        if (minutes <= 0) return seconds.ToString("00.00");
        return minutes.ToString("00") + ":" + seconds.ToString("00.00");
    }
}
