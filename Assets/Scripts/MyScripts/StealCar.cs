using UnityEngine;

public class StealCar : PurchasableInteractable
{
    [SerializeField] private string getDownOfTheCarInteractionString;
    [SerializeField] private Transform playerPositionParent;

    [SerializeField] private Behaviour[] componentsAI;
    [SerializeField] private Behaviour[] componentsPlayer;
    [SerializeField] private GameObject[] gameObjectsPlayer;


    private bool inTheCar = false;
    private void Start()
    {
        SetComponents();
    }
    protected override bool ExecuteInteraction(GameObject user)
    {
        inTheCar = !inTheCar; 

        GameManager.Instance.SetInteractParent(inTheCar, playerPositionParent);
        GameManager.Instance.EnablePlayer(!inTheCar);

        SetComponents();

        return true;
    }

    //Dynamic UI Prompt management
    public override string GetInteractionText()
    {
        if (inTheCar) return getDownOfTheCarInteractionString;

        return interactString;
    }
    private void SetComponents()
    {
        //ai
        for (int i = 0; i < componentsAI.Length; i++) componentsAI[i].enabled = !inTheCar;

        //player
        for (int i = 0; i < componentsPlayer.Length; i++) componentsPlayer[i].enabled = inTheCar;
        for (int i = 0; i < gameObjectsPlayer.Length; i++) gameObjectsPlayer[i].SetActive(inTheCar);
    }
}
