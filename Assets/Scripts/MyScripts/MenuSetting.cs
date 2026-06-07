using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MenuAsset : IPickableItem
{
    public string name;
    public Sprite image;

    public string Name => name;
    public Sprite Image => image;
}

[CreateAssetMenu(fileName = "MenuSetting", menuName = "Scriptable Objects/MenuSetting")]
public class MenuSetting : ScriptableObject
{
    [Header("Selections")]
    public int selectedCarIndex;
    public int selectedMapIndex;
    public int maxEnemies;
    public int currentEnemies;

    [Header("Available Options")]
    public MenuAsset[] carOptions;
    public MenuAsset[] mapOptions;
}
