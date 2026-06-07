using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneLoader : MonoBehaviour
{
    public void LoadScene(string toLoad)
    {
        SceneManager.LoadScene(toLoad);
    }
    public void ExitApplication()
    {
        Application.Quit();
    }
}
