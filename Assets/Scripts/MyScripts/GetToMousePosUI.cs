using UnityEngine;
using UnityEngine.InputSystem;
public class GetToMousePosUI : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas parentCanvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector2 localPoint;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, mouseScreenPosition, parentCanvas.worldCamera, out localPoint);

        rectTransform.anchoredPosition = localPoint;
    }
}
