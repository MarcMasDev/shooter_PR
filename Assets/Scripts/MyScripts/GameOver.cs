using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private CharacterBlackboard player;
    [SerializeField] private Button restartButton;
    [SerializeField] private bool winEnabled = true;

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

        if (m_ActiveEnemies != null)
        {
            foreach (var enemy in m_ActiveEnemies)
            {
                if (enemy != null) enemy.OnDeath -= () => OnEnemyDied(enemy);
            }
        }
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
        // Subscribe to that specific enemy's death
        enemy.OnDeath += () => OnEnemyDied(enemy);
    }

    private void OnEnemyDied(CharacterBlackboard enemy)
    {
        m_ActiveEnemies.Remove(enemy);

        if (m_ActiveEnemies.Count <= 0)
        {
            WinScreenTrigger();
        }
    }

    private void GameOverTrigger()
    {
        gameOverScreen.SetActive(true);
        EnableMenuControlInput();
    }

    private void WinScreenTrigger()
    {
        if (!winEnabled) return;

        winScreen.SetActive(true);
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
        // --- Reset del Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Obtiene el índice de la escena actual y la vuelve a cargar
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);


    }
}
