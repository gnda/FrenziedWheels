using UnityEngine;
using Random = UnityEngine.Random;

public class Car : MonoBehaviour
{
    [SerializeField] public Material[] Palettes;

    private void Start()
    {
        // Randomize car color
        if (GetComponent<Player>() == null)
            transform.Find("Car").Find("CarBody").
                    GetComponent<MeshRenderer>().sharedMaterial =
                        Palettes[Random.Range(0, Palettes.Length)];
    }
}