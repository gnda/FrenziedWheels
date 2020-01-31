using UnityEngine;
using SDD.Events;

public class PlayerController : SimpleGameStateObserver, IEventHandler
{
	private Car currentCar;
	
	#region MonoBehaviour lifecycle
	// Use this for initialization
	void Start ()
	{
		currentCar = GetComponent<Car>();
	}

	// Update is called once per frame
	void Update () {
		if (GameManager.Instance && !GameManager.Instance.IsPlaying) return;

		float hInput = Input.GetAxis("Horizontal");
		float vInput = Input.GetAxis("Vertical");
		bool spaceBar = Input.GetButton("Jump");

		if (vInput > 0f)
		{
			currentCar.CurrentSpeed += (currentCar.AccelerationRate * Time.deltaTime);
		}
		else
		{
			currentCar.CurrentSpeed -= (currentCar.DecelerationRate * Time.deltaTime);
		}

		if (vInput < 0f)
		{
			currentCar.CurrentSpeed -= (currentCar.DecelerationRate * Time.deltaTime * 2);
		}

		transform.Translate(0f, 0f, currentCar.CurrentSpeed * Time.deltaTime);
		transform.Rotate( new Vector3(0,50 * hInput * Time.deltaTime,0));
	}
	#endregion
}