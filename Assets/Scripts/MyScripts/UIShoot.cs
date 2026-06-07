using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIShoot : MonoBehaviour
{
    [Header("Configuración del Raycast")]
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private float fireRate = 0.1f;
    bool isFiring = false;

    [Header("Configuración de Efectos Visuales (VFX)")]
    [SerializeField] private GameObject vfxPrefab;

    [Header("Configuración de Audio")]
    [SerializeField] protected AudioItem[] audioItems;

    private InputAction m_ShootAction;
    private Camera mainCamera;
    private RectTransform rectTransform;
    private GameObject toDestroy = null;

    void Start()
    {
        mainCamera = GameManager.Instance.GetMainCamera();
        rectTransform = GetComponent<RectTransform>();
        m_ShootAction = GameManager.Instance.GetInput().actions["Shoot"];
        m_ShootAction.Enable();
    }

    void OnEnable() => m_ShootAction?.Enable();
    void OnDisable() => m_ShootAction?.Disable();

    void Update()
    {
        if (m_ShootAction != null && m_ShootAction.IsPressed())
        {
            if (!isFiring) StartCoroutine(FullAutoRoutine());
        }
        else isFiring = false;
    }

    private void ProcessAutomaticShot()
    {
        Vector2 uiScreenPosition = rectTransform.position;

        // Convertir la posición calculada de la interfaz al mundo
        Ray ray = mainCamera.ScreenPointToRay(uiScreenPosition);

        // Reproducir el sonido de disparo mediante el sistema del AudioManager
        if (audioItems != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlayOneShootFromArray(audioItems, SoundType.shoot);
        }

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitLayers))
        {
            //Crear el efecto visual en el punto de impacto, orientado según la inclinación de la superficie
            if (vfxPrefab != null)
            {
                // Evita el z-figthing
                Vector3 spawnPos = hit.point + (hit.normal * 0.01f);
                Quaternion spawnRot = Quaternion.LookRotation(hit.normal);

                //Crea el efecto
                GameObject vfxInstance = Instantiate(vfxPrefab, spawnPos, spawnRot);
                if (toDestroy != null) Destroy(toDestroy);
                toDestroy = vfxInstance;
            }

            if (hit.collider.TryGetComponent(out UIAction scriptAccion))
            {
                // Si el objeto tiene el script, ejecutamos su función
                scriptAccion.PerformAction();
            }
        }
    }
    private IEnumerator FullAutoRoutine()
    {
        isFiring = true;

        while (isFiring)
        {
            ProcessAutomaticShot();
            yield return new WaitForSeconds(1f / fireRate);
        }
        isFiring = false;
    }
}
