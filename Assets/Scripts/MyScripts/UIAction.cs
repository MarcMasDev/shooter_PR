using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class UIAction : MonoBehaviour
{
    public enum UIActionType
    {
        loadScene,
        exit,
        activateGameobjects,
        changeVolume,
        changeResolution,
        changeQuality,
        window
    }

    //ACTIONS
    [SerializeField] private UIActionType[] actionTypes;

    //LOAD SCENE
    [ShowIf("IncludesLoadScene")] [SerializeField] private string loadSceneName = "";

    //ACTIVE / DEACTIVE
    [ShowIf("IncludesActiveGameObject")] [SerializeField] private GameObject[] toActivate;
    [ShowIf("IncludesActiveGameObject")] [SerializeField] private GameObject[] toDeactivate;
    [ShowIf("IncludesActiveGameObject")] [SerializeField] private bool deactivateSelf = false;

    //VOLUME
    [ShowIf("IncludesVolume")] [SerializeField] private AudioMixer mixer;
    [ShowIf("IncludesVolume")] [SerializeField] private string volumeParameter = "MusicVolume";
    [ShowIf("IncludesVolume")] [SerializeField] private TMP_Text volumeText;
    [ShowIf("IncludesVolume")] [SerializeField] private int dir;
    private float[] volumes = { 0f, 0.25f, 0.5f, 0.75f, 1f };

    // RESOLUTION
    [ShowIf("IncludesResolution")] [SerializeField] private int resolutionIndex;
    [ShowIf("IncludesResolution")] [SerializeField] private TMP_Text resolutionText;
    private Resolution[] resolutions;

    // QUALITY
    [ShowIf("IncludesQuality")] [SerializeField] private int qualityIndex;
    [ShowIf("IncludesQuality")] [SerializeField] private TMP_Text qualityText;

    //WINDOWED
    [ShowIf("IncludesWindowed")][SerializeField] private int windowType;
    private bool IncludesLoadScene => IncludesAction(UIActionType.loadScene);
    private bool IncludesActiveGameObject => IncludesAction(UIActionType.activateGameobjects);
    private bool IncludesVolume => IncludesAction(UIActionType.changeVolume);
    private bool IncludesResolution => IncludesAction(UIActionType.changeResolution);
    private bool IncludesQuality => IncludesAction(UIActionType.changeQuality);
    private bool IncludesWindowed => IncludesAction(UIActionType.window);

    private bool attacked = false;
    private void OnEnable()
    {
        if (IncludesResolution)
        {
            InitializeResolution();
        }
        if (IncludesQuality)
        {
            InitializeQuality();
        }
        if (IncludesVolume)
        {
            ChangeVolume(0);
        }
    }
    public void PerformAction()
    {
        attacked = !attacked;

        if (attacked)
        {
            if (IncludesAction(UIActionType.exit))
            {
                Application.Quit();
            }

            if (IncludesLoadScene)
            {
                SceneManager.LoadScene(loadSceneName);
            }

            if (IncludesVolume)
            {
                ChangeVolume(dir);
            }

            if (IncludesResolution)
            {
                SetResolution();
            }

            if (IncludesQuality)
            {
                SetQuality();
            }

            if (IncludesWindowed)
            {
                SetScreenMode();
            }
        }


        if (IncludesActiveGameObject)
        {
            for (int i = 0; i < toActivate.Length; i++)
            {
                toActivate[i].SetActive(attacked);
            }

            for (int i = 0; i < toDeactivate.Length; i++)
            {
                toDeactivate[i].SetActive(!attacked);
            }

            if (attacked && deactivateSelf) gameObject.SetActive(false);
        }
    }
    private bool IncludesAction(UIActionType actionType)
    {
        for (int i = 0; i < actionTypes.Length; i++)
        {
            if (actionTypes[i] == actionType) return true;
        }
        return false;
    }

    #region Volume
    private void ChangeVolume(int direction)
    {
        int index = GetCurrentVolumeIndex();

        index += direction;
        index = Mathf.Clamp(index, 0, volumes.Length - 1);

        float volume = volumes[index];

        float db = volume <= 0f ? -80f : Mathf.Log10(volume) * 20f;

        mixer.SetFloat(volumeParameter, db);

        volumeText.text = $"{Mathf.RoundToInt(volume * 100f)}%";
    }

    private int GetCurrentVolumeIndex()
    {
        if (!mixer.GetFloat(volumeParameter, out float db)) return 0;

        float currentVolume = db <= -80f ? 0f : Mathf.Pow(10f, db / 20f);

        int closestIndex = 0;
        float closestDistance = Mathf.Abs(volumes[0] - currentVolume);

        for (int i = 1; i < volumes.Length; i++)
        {
            float distance = Mathf.Abs(volumes[i] - currentVolume);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    #endregion

    #region Resolution

    private void InitializeResolution()
    {
        resolutions = Screen.resolutions;
        resolutionText.text = $"{resolutions[resolutionIndex].width} x {resolutions[resolutionIndex].height}";
    }

    private void SetResolution()
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width,resolution.height,Screen.fullScreen);
    }

    #endregion

    #region Quality

    private void InitializeQuality()
    {
        Debug.Log($"Quality count: {QualitySettings.names.Length}");
        Debug.Log($"Index: {qualityIndex}");
        qualityText.text = QualitySettings.names[qualityIndex];

    }

    private void SetQuality()
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
    }

    #endregion

    #region Window
    private void SetScreenMode()
    {
        switch (windowType)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }
    #endregion
}
