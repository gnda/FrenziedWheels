using System.Collections;
using UnityEngine;
using SDD.Events;

public class PlayerController : SimpleGameStateObserver, IEventHandler
{
	private Car currentCar;
	private Circuit currentCircuit;
	private bool IsRotating;
	
	#region MonoBehaviour lifecycle
	// Use this for initialization
	void Start ()
	{
		currentCar = GetComponent<Car>();
		currentCircuit = GameManager.Instance.CurrentCircuit;
	}

	// Update is called once per frame
	void Update () {
		if (GameManager.Instance && !GameManager.Instance.IsPlaying) return;

		float hInput = Input.GetAxis("Horizontal");
		float vInput = Input.GetAxis("Vertical");

		if (vInput > 0f)
		{
			currentCar.Accelerate();
		}
		else if (currentCar.CurrentSpeed > 0f)
		{
			if (vInput < 0f)
			{
				currentCar.Decelerate(3f);
			}
			else
			{
				currentCar.Decelerate();
			}
		}
		
		transform.Translate(0f, 0f, currentCar.CurrentSpeed * Time.deltaTime); 
		transform.Rotate(new Vector3(0,80 * hInput * Time.deltaTime,0));
	}
	
	#endregion
}