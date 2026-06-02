using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    [SerializeField] private Transform player;
    [SerializeField] private Camera cameraMain;
    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;

        if (cameraMain == null) cameraMain = Camera.main;
    }
    public Transform GetPlayerTransform() => player;
    public void EnablePlayer(bool enable)
    {
        player.gameObject.SetActive(enable);
        cameraMain.gameObject.SetActive(enable);
    }
}
