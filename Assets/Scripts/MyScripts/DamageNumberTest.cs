using UnityEngine;
using DamageNumbersPro;
public class DamageNumberTest : MonoBehaviour
{
    //Assign prefab in inspector.
    public DamageNumber numberPrefab;
    public RectTransform rectParent;

    void Update()
    {
        DamageNumber damageNumber = numberPrefab.SpawnGUI(rectParent, Vector2.zero, Random.Range(1, 100));
    }
}
