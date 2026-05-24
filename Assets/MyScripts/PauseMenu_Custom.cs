using UnityEngine;

public class PauseMenu_Custom : MonoBehaviour
{
    private bool isPaused = false;
    private CanvasGroup cg;
    private void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        ShowCG(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    private void Resume()
    {
        ShowCG(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    private void ShowCG(bool show)
    {
        cg.interactable = show;
        cg.blocksRaycasts = show;

        if (show) cg.alpha = 1;
        else cg.alpha = 0;
    }
}
