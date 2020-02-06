using UnityEngine;
using Random = UnityEngine.Random;

public class Car : MonoBehaviour
{
    public float CurrentSpeed { get; set; }
    public bool IsGrounded { get; set; }

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

        /*if (!IsGrounded)
        {
            CurrentSpeed *= 0.98f;
        }*/
    }
    
    private void OnCollisionStay(Collision other)
    {
        IsGrounded = true;
        
        /*if (other.collider.CompareTag("Ground") && CurrentSpeed > 0)
        {
            Decelerate(0.5f);
        }*/
    }

    private void OnCollisionExit(Collision other)
    {
        IsGrounded = false;
    }
}