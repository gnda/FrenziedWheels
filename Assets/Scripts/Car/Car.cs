using UnityEngine;
using Random = UnityEngine.Random;

public class Car : MonoBehaviour
{
    [SerializeField] public Material[] Palettes;

    private void Start()
    {
        // Randomize car color
        if (GetComponent<Player>() == null)
            transform.Find("SkyCar").Find("SkyCarBody").
                    GetComponent<MeshRenderer>().sharedMaterial =
                        Palettes[Random.Range(0, Palettes.Length)];
    }
}