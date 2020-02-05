using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Car : MonoBehaviour
{
    public float CurrentSpeed { get; set; }

    [field: SerializeField] public float MaxSpeed { get; set; } = 0f;
    [field: SerializeField] public float AccelerationRate { get; set; } = 0f;
    [field: SerializeField] public float DecelerationRate { get; set; } = 0f;

    [SerializeField] public Material[] Palettes;

    private void Start()
    {
        // Randomize car color
        if (GetComponent<Player>() == null)
            transform.Find("Frame").GetComponent<MeshRenderer>().sharedMaterial =
                Palettes[Random.Range(0, Palettes.Length)];
    }

    public void Accelerate()
    {
        CurrentSpeed += (AccelerationRate * (1-(CurrentSpeed/MaxSpeed)) * Time.deltaTime);
    }
    
    public void Decelerate(float multiply = 1f)
    {
        CurrentSpeed -= (DecelerationRate * Time.deltaTime * multiply);
    }
    
    private void Update()
    {
        Transform transf = transform;
        // Limit speed
        CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, MaxSpeed);
        // Block z rotation
        transform.Rotate(0,0, -transf.eulerAngles.z, Space.Self);
        // Limit vertical position
        Vector3 currentPos = transf.position;
        transform.position = new Vector3(currentPos.x, 
            Mathf.Clamp(currentPos.y, -30f, 7f), currentPos.z);
    }
}