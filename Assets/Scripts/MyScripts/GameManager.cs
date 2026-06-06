using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    [SerializeField] private Transform player;
    [SerializeField] private Transform playerInteractor;
    [SerializeField] private Camera cameraMain;
    [SerializeField] private PlayerInput m_PlayerInput;
    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;

        if (m_PlayerInput == null) m_PlayerInput = GetComponent<PlayerInput>();
        if (cameraMain == null) cameraMain = Camera.main;
        SetInteractParent(false, player);
    }
    public Transform GetPlayerTransform()
    {
        if (player.gameObject.activeSelf) return player;
        return playerInteractor;
    }
    public void EnablePlayer(bool enable)
    {
        if (enable && !player.gameObject.activeSelf) player.position = playerInteractor.position;

        player.gameObject.SetActive(enable);
        cameraMain.gameObject.SetActive(enable);
    }

    public void SetInteractParent(bool enable, Transform playerPositionParent)
    {
        if (enable) playerInteractor.SetParent(playerPositionParent);
        else playerInteractor.SetParent(player);
    }

    public PlayerInput GetInput()
    {
        return m_PlayerInput;
    }

    public Camera GetMainCamera() => cameraMain;
}
