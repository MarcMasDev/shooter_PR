using TMPro;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text tooltip;
    private void Awake()
    {
        Show("");
    }
    public void Show(string txt)
    {
        if (txt == string.Empty) ShowTooltip(txt);

        text.text = txt;
    }
    public void ShowTooltip(string txt)
    {
        tooltip.text = txt;
    }
}
