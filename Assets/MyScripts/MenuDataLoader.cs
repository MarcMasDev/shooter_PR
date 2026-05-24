using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuDataLoader : MonoBehaviour
{
    [SerializeField] private MenuSetting menuSettings;

    [Header("PickLists")]
    [SerializeField] private PickList mapPickList;
    [SerializeField] private PickList carPickList;

    [Header("Enemies")]
    [SerializeField] private TMP_Text enemiesDisplay;
    [SerializeField] private Button addEnemy;
    [SerializeField] private Button removeEnemy;

    private void Awake()
    {
        InitEnemies(); 
        InitPickLists();
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    //Picklists
    private void InitPickLists()
    {
        mapPickList.Init(new List<IPickableItem>(menuSettings.mapOptions),menuSettings.selectedMapIndex, OnMapChanged);
        carPickList.Init(new List<IPickableItem>(menuSettings.carOptions),menuSettings.selectedCarIndex, OnCarChanged);
    }
    private void OnMapChanged(int index)
    {
        menuSettings.selectedMapIndex = index;
    }
    private void OnCarChanged(int index)
    {
        menuSettings.selectedCarIndex = index;
    }

    //ENEMIES
    private void InitEnemies()
    {
        addEnemy.onClick.AddListener(AddEnemy);
        removeEnemy.onClick.AddListener(RemoveEnemy);
        UpdateEnemies();
    }
    public void AddEnemy()
    {
        if (menuSettings.currentEnemies >= menuSettings.maxEnemies) return;

        menuSettings.currentEnemies++;
        UpdateEnemies();
    }
    public void RemoveEnemy()
    {
        if (menuSettings.currentEnemies <= 0) return;

        menuSettings.currentEnemies--;
        UpdateEnemies();
    }
    private void UpdateEnemies()
    {
        menuSettings.currentEnemies = Mathf.Clamp(menuSettings.currentEnemies, 0, menuSettings.maxEnemies);
        EnableEnemyButtons();
        enemiesDisplay.text = menuSettings.currentEnemies.ToString();
    }
    private void EnableEnemyButtons()
    {
        removeEnemy.gameObject.SetActive(true);
        addEnemy.gameObject.SetActive(true);

        if (menuSettings.currentEnemies <= 0) removeEnemy.gameObject.SetActive(false);
        else if (menuSettings.currentEnemies >= menuSettings.maxEnemies) addEnemy.gameObject.SetActive(false);
    }
}
