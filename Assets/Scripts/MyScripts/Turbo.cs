using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif
public class Turbo : MonoBehaviour
{
    [Header("Turbo Settings")]
    public float maxTurbo = 100f;
    [SerializeField] private float turboDrainSpeed = 30f;
    [SerializeField] private float turboRechargeSpeed = 20f;

    [Header("Movement")]
    [SerializeField] private float turboForce = 20f;
    private Rigidbody rb;

    [Header("Particles")]
    [SerializeField] private ParticleSystem turboParticles;

    private float currentTurbo = 0;
    private bool canBoost = true;

    private InputAction m_HandbrakeAction;

    private void Start()
    {
        m_HandbrakeAction = GameManager.Instance.GetInput().actions["Handbrake"];
        rb = GetComponent<Rigidbody>();
        currentTurbo = maxTurbo;
        SetParticles(false);
    }

    private void Update()
    {
        if (currentTurbo >= maxTurbo) canBoost = true;

        bool isSpacePressed = false;
        bool wasSpaceReleased = false;

        if (m_HandbrakeAction != null)
        {
            isSpacePressed = m_HandbrakeAction.IsPressed();
            wasSpaceReleased = m_HandbrakeAction.WasReleasedThisFrame();
        }

        if (isSpacePressed && canBoost) Boost();
        else RecoverTurbo();

        if (wasSpaceReleased || currentTurbo <= 0f) canBoost = false;
    }

    private void Boost()
    {
        currentTurbo -= turboDrainSpeed * Time.deltaTime;
        currentTurbo = Mathf.Clamp(currentTurbo, 0f, maxTurbo);

        rb.AddForce(transform.forward * turboForce, ForceMode.Acceleration);

        SetParticles(true);
    }

    private void RecoverTurbo()
    {
        currentTurbo += turboRechargeSpeed * Time.deltaTime;
        currentTurbo = Mathf.Clamp(currentTurbo, 0f, maxTurbo);

        SetParticles(false);
    }

    public float GetTurbo()
    {
        return currentTurbo;
    }
    public bool CanTurbo()
    {
        return canBoost;
    }
    private void SetParticles(bool activated)
    {
        if (turboParticles == null) return;

        if (activated) turboParticles.Play();
        else turboParticles.Stop();

        turboParticles.gameObject.SetActive(activated);
    }
}
