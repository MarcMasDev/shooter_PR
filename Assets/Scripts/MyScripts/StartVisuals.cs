using UnityEngine;

public class StartVisuals : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 0;
    }

    public void StartGame()
    {
        Time.timeScale = 1;
    }
}
