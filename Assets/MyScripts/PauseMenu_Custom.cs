using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro; // Necesario para los Dropdowns de TextMeshPro
using UnityEngine.SceneManagement;

public class PauseMenu_Custom : MonoBehaviour
{
    [Header("Menu State")]
    private bool isPaused = false;
    private CanvasGroup cg;
    private InputAction pauseAction;

    [Header("Audio Settings")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string musicVolumeParameter = "MusicVol";
    [SerializeField] private string sfxVolumeParameter = "SFXVol";
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider volumeSfxSlider;

    [Header("Video Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;

    private Resolution[] resolutions;

    private void Start()
    {
        cg = GetComponent<CanvasGroup>();

        // Inicializar el input (Asegúrate de que GameManager no sea nulo antes de esto)
        if (GameManager.Instance != null)
        {
            pauseAction = GameManager.Instance.GetInput().actions["Pause"];
        }

        // Configurar los valores iniciales de la UI
        InitializeSettingsUI();

        // Asegurarnos de que el menú empiece oculto
        Resume();
    }

    private void Update()
    {
        if (pauseAction != null && pauseAction.triggered)
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


    // LÓGICA DE PAUSA

    public void Pause()
    {
        GameManager.Instance.EnableInput(false);
        ShowCG(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        GameManager.Instance.EnableInput(true);
        ShowCG(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ShowCG(bool show)
    {
        cg.interactable = show;
        cg.blocksRaycasts = show;
        cg.alpha = show ? 1 : 0;
    }


    // INICIALIZACIÓN DE UI
    private void InitializeSettingsUI()
    {
        // Volumen
        if (mixer.GetFloat(musicVolumeParameter, out float currentDb))
        {
            // Convertir decibelios de vuelta a un valor lineal para el slider (0.0001 a 1)
            float currentVolume = Mathf.Pow(10f, currentDb / 20f);
            musicVolumeSlider.value = currentVolume;
        }
        if (mixer.GetFloat(sfxVolumeParameter, out float sfxCurrentDb))
        {
            // Convertir decibelios de vuelta a un valor lineal para el slider (0.0001 a 1)
            float currentVolume = Mathf.Pow(10f, sfxCurrentDb / 20f);
            volumeSfxSlider.value = currentVolume;
        }
        // Asignar el evento automáticamente desde el código
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        volumeSfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // Resoluciones
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // Verificar si esta resolución de la lista es la actual de la pantalla
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        // Calidad
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
        qualityDropdown.onValueChanged.AddListener(SetQuality);

        // Modo de Pantalla (Window)
        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(new List<string> { "Fullscreen", "Fullscreen Window", "Windowed" });
        screenModeDropdown.value = GetCurrentScreenModeIndex();
        screenModeDropdown.RefreshShownValue();
        screenModeDropdown.onValueChanged.AddListener(SetScreenMode);
    }


    // MÉTODOS DE OPCIONES (Llamados por la UI)

    public void SetMusicVolume(float volume)
    {
        float clampedVol = Mathf.Clamp(volume, 0.0001f, 1f);
        float db = Mathf.Log10(clampedVol) * 20f;
        mixer.SetFloat(musicVolumeParameter, db);
    }
    public void SetSFXVolume(float volume)
    {
        float clampedVol = Mathf.Clamp(volume, 0.0001f, 1f);
        float db = Mathf.Log10(clampedVol) * 20f;
        mixer.SetFloat(sfxVolumeParameter, db);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
    }

    public void SetScreenMode(int modeIndex)
    {
        switch (modeIndex)
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

    private int GetCurrentScreenModeIndex()
    {
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.ExclusiveFullScreen: return 0;
            case FullScreenMode.FullScreenWindow: return 1;
            case FullScreenMode.Windowed: return 2;
            default: return 0;
        }
    }

    // OTRAS ACCIONES (Cargar / Salir)
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f; // Restaurar el tiempo antes de cambiar de escena
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}