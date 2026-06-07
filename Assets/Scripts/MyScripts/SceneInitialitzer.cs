using UnityEngine;

public class SceneInitialitzer : MonoBehaviour
{
    [SerializeField] private MenuSetting menuSettings;
    [SerializeField] private Race[] races;

    private void Awake()
    {
        InitScene();
    }

    private void InitScene()
    {
        for (int i = 0; i < races.Length; i++)
        {
            races[i].gameObject.SetActive(i == menuSettings.selectedMapIndex);
        }

        Race selectedRace = races[menuSettings.selectedMapIndex];

        for (int i = 0; i < selectedRace.players.Length; i++)
        {
            selectedRace.players[i].SetActive(i == menuSettings.selectedCarIndex);
        }

        for (int i = 0; i < menuSettings.currentEnemies; i++)
        {
            selectedRace.enemies[i].SetActive(true);
        }
    }
}
