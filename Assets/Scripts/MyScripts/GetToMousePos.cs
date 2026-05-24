using UnityEngine;
using UnityEngine.InputSystem;

public class GetToMousePos : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;

    [Header("Aim")]
    [SerializeField] private LayerMask aimLayers;
    [SerializeField] private float maxDistance = 1000f;
    [SerializeField] private float smoothSpeed = 20f;

    private void Update()
    {
        //Mouse position using NEW Input System
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        //Create ray from camera through mouse
        Ray ray = cam.ScreenPointToRay(mousePosition);

        Vector3 targetPoint;

        //Raycast into world
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, aimLayers))
        {
            targetPoint = hit.point;
        }
        else
        {
            //Fallback point if nothing hit
            targetPoint = ray.GetPoint(100f);
        }

        //Smooth aim target movement
        transform.position = Vector3.Lerp(transform.position,targetPoint, Time.deltaTime * smoothSpeed);
    }
}
