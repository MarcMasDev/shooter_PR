using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class CarStatusDisplayerUI : MonoBehaviour
{
    private CarController carController;
    private Turbo turboController;

    [Header("Speed Settings")]
    [SerializeField] private Slider speedSlider;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private float speedMultiplier = 2.5f;

    [Header("Turbo Settings")]
    [SerializeField] private Slider turboSlider;
    [SerializeField] private TMP_Text turboText;
    [SerializeField] private Color turboColor;
    [SerializeField] private Image colorBg;
    [SerializeField] private Image colorSlider;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        carController = player.GetComponent<CarController>();
        turboController = player.GetComponent<Turbo>();

        InitSpeedSlider();
        InitBoostSlider();
    }

    private void Update()
    {
        if (carController != null) UpdateSpeed(carController.CurrentSpeed * speedMultiplier);
        if (turboController != null) UpdateBoost(turboController.GetTurbo());
    }

    private void InitSpeedSlider()
    {
        if (speedSlider != null)
        {
            speedSlider.minValue = 0f;
            speedSlider.maxValue = carController.MaxSpeed * speedMultiplier;
        }
    }
    private void UpdateSpeed(float speed)
    {
        speedText.text = speed.ToString("0");

        if (speedSlider != null)
        {
            speedSlider.value = speed;
        }
    }

    private void InitBoostSlider()
    {
        if (turboSlider != null && turboController != null)
        {
            turboSlider.minValue = 0f;
            turboSlider.maxValue = turboController.maxTurbo;
        }
    }

    private void UpdateBoost(float turbo)
    {
        turboText.text = Mathf.RoundToInt(turbo) + "%";

        if (turboSlider != null) turboSlider.value = turbo;

        if (turboController.CanTurbo())
        {
            colorBg.color = turboColor;
            colorSlider.color = turboColor;
        }
        else
        {
            colorBg.color = Color.white;
            colorSlider.color = Color.white;
        }
    }
}
