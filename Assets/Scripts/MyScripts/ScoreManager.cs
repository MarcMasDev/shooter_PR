using TMPro;
using UnityEngine;
using DamageNumbersPro;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _instance;
    public static ScoreManager Instance { get { return _instance; } }

    [Header("Score")]
    [SerializeField] private DamageNumber floatingTextScore;
    [SerializeField] private DamageNumber floatingTextDecreaseScore;
    [SerializeField] private TMP_Text scoreDisplayer;
    [SerializeField] private RectTransform numberPointsSpawnPoint;

    [Header("Score Settings")]
    [SerializeField] private int pointsPerImpact = 10;
    [SerializeField] private int pointsPerHeadshot = 100;
    [SerializeField] private int pointsPerKill = 50;
    public bool doublePoints = false;

    [Tooltip("Starting points")]
    [SerializeField] private int points = 500;


    [Header("Kills")]
    [SerializeField] private DamageNumber floatingTextKills;
    [SerializeField] private TMP_Text killsDisplayer;
    [SerializeField] private RectTransform numberKillsSpawnPoint;
    private int currentKills = 0;

    void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;
        scoreDisplayer.text = points.ToString("N0");
        killsDisplayer.text = currentKills.ToString("N0");
    }
    public void AddPoints(HitboxType hitboxType, bool death = false)
    {
        int currentPoints = 0;
        int pointsMultiplier = 1;
        if (doublePoints) pointsMultiplier = 2;

        if (!death) currentPoints = pointsPerImpact * pointsMultiplier;
        else
        {
            switch (hitboxType)
            {
                case HitboxType.Head:
                    currentPoints = pointsPerHeadshot * pointsMultiplier;
                    break;
                default:
                    currentPoints = pointsPerKill * pointsMultiplier;
                    break;
            }
        }

        points += currentPoints;
        scoreDisplayer.text = points.ToString("N0");
        SpawnPoints(currentPoints);
    }

    public bool CheckPoints(int amount) => amount <= points;
    public void DecreasePoints(int amount)
    {
        points -= amount;
        scoreDisplayer.text = points.ToString("N0");
        SpawnNegativePoints(-amount);
    }

    public void AddKill()
    {
        currentKills++;
        killsDisplayer.text = currentKills.ToString("N0");
        SpawnKills(1);
    }

    public int GetCurrentKills() => currentKills;

    private void SpawnPoints(int pointsToShow) => floatingTextScore.SpawnGUI(numberPointsSpawnPoint, Vector2.zero, pointsToShow);
    private void SpawnNegativePoints(int pointsToShow) => floatingTextDecreaseScore.SpawnGUI(numberPointsSpawnPoint, Vector2.zero, pointsToShow);
    private void SpawnKills(int killsToShow) => floatingTextKills.SpawnGUI(numberKillsSpawnPoint, Vector2.zero, killsToShow);
}
