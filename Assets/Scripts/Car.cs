using System;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float CurrentSpeed { get; set; } = 0f;

    [field: SerializeField] public float MaxSpeed { get; set; } = 0f;
    [field: SerializeField] public float AccelerationRate { get; set; } = 0f;
    [field: SerializeField] public float DecelerationRate { get; set; } = 0f;

    private void Update()
    {
        // Limit speed
        CurrentSpeed = Mathf.Clamp(CurrentSpeed, 0f, MaxSpeed);
    }
}