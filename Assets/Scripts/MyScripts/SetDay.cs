using UnityEngine;

public class SetDay : MonoBehaviour
{
    [SerializeField] private GameObject dayLight;
    [SerializeField] private GameObject nightLight;
    [SerializeField] private bool night = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dayLight.SetActive(!night);
        nightLight.SetActive(night);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            dayLight.SetActive(!dayLight.activeSelf);
            nightLight.SetActive(!nightLight.activeSelf);
        }
    }
}
