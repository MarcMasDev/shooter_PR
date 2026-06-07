using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public interface IPickableItem
{
    string Name { get; }
    Sprite Image { get; }
}
public class PickList : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image image;
    [SerializeField] private Button next;
    [SerializeField] private Button previous;

    private List<IPickableItem> items;
    private int index;
    private Action<int> onIndexChanged;

    public void Init(List<IPickableItem> items, int startIndex = 0, Action<int> onIndexChanged = null)
    {
        this.items = items;
        index = startIndex;
        this.onIndexChanged = onIndexChanged;

        next.onClick.AddListener(GetNext);
        previous.onClick.AddListener(GetPrevious);

        UpdateVisuals();
    }

    private void GetNext()
    {
        index = (index + 1) % items.Count;
        UpdateVisuals();
    }

    private void GetPrevious()
    {
        index = (index - 1 + items.Count) % items.Count;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        var item = items[index];
        nameText.text = item.Name;
        image.sprite = item.Image;
        onIndexChanged?.Invoke(index);
    }

    public int GetIndex() => index;
}
