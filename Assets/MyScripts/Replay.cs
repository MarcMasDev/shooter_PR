using UnityEngine;
using UnityEngine.UI;

public class Replay : MonoBehaviour
{
    [SerializeField] private GhostPlayer[] replayPlayers;
    private Button replayButton;
    [SerializeField] private GameObject menu;
    private void Start()
    {
        replayButton = GetComponent<Button>();
        replayButton.onClick.AddListener(OnClick);
    }
    private void Update()
    {
        replayButton.interactable = AllPlayersEnded();
    }
    public void OnClick()
    {
        for (int i = 0; i < replayPlayers.Length; i++)
        {
            if (replayPlayers[i].gameObject.activeSelf) replayPlayers[i].StartGhost();
        }
        menu.SetActive(false);
    }
    private bool AllPlayersEnded()
    {
        for(int i = 0; i < replayPlayers.Length; i++)
        {
            if (replayPlayers[i].gameObject.activeSelf && replayPlayers[i].ghostData == null)
            {
                return false;
            }
        }
        return true;
    }
}
