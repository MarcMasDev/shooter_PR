using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private CharacterBlackboard player;
    [SerializeField] private Button restartButton;

    //Win = all enemies death
    private List<CharacterBlackboard> m_ActiveEnemies = new List<CharacterBlackboard>();

    private void OnEnable()
    {
        player.OnDeath += GameOverTrigger;
        restartButton.onClick.AddListener(RestartGame);
    }
    private void OnDisable()
    {
        player.OnDeath -= GameOverTrigger;
        restartButton.onClick.RemoveListener(RestartGame);
    }
    private void Awake()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            m_ActiveEnemies.Add(enemies[i].GetComponent<CharacterBlackboard>());
        }
    }
    public void RegisterEnemy(CharacterBlackboard enemy)
    {
        m_ActiveEnemies.Add(enemy);
    }

    private void GameOverTrigger()
    {
        GameManager.Instance.EnableInput(false);
        gameOverScreen.SetActive(true);
        EnableMenuControlInput();
    }

    private void EnableMenuControlInput()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None; //Desbloquea el cursor para que se mueva libremente
        Cursor.visible = true; //Hace que el cursor sea visible
    }
    public void RestartGame()
    {
        Time.timeScale = 1;
        GameManager.Instance.EnableInput(true);

        // --- Reset del Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Obtiene el índice de la escena actual y la vuelve a cargar
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
