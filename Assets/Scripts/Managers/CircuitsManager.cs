using System.Collections;
using UnityEngine;
using SDD.Events;

public class CircuitsManager : Manager<CircuitsManager> {

	[Header("CircuitsManager")]
	
	#region Circuits & current circuit management
	
	private int currentCircuitIndex;
	private GameObject currentCircuitGO;


	[SerializeField] private GameObject[] circuitsPrefabs;
	
	#endregion

	#region Manager implementation
	protected override IEnumerator InitCoroutine()
	{
		yield break;
	}
	#endregion

	#region Events' subscription
	public override void SubscribeEvents()
	{
		base.SubscribeEvents();
		EventManager.Instance.AddListener<GoToNextCircuitEvent>(GoToNextCircuit);
	}

	public override void UnsubscribeEvents()
	{
		base.UnsubscribeEvents();
		EventManager.Instance.RemoveListener<GoToNextCircuitEvent>(GoToNextCircuit);
	}
	#endregion

	#region Level flow
	void Reset()
	{
		Player.PlayerCount = 0;
		// Detach Camera from player
		Camera camera = FindObjectOfType<Camera>();
		camera.transform.parent = null;

		EventManager.Instance.Raise(new CircuitHasBeenDestroyedEvent());
		Destroy(currentCircuitGO);
	}

	void InstantiateCircuit()
	{
		currentCircuitIndex = Mathf.Max(currentCircuitIndex, 0) % circuitsPrefabs.Length;
		MusicLoopsManager.Instance.PlayMusic(currentCircuitIndex + 1);
		currentCircuitGO = Instantiate(circuitsPrefabs[currentCircuitIndex]);
	}

	private IEnumerator GoToNextCircuitCoroutine()
	{
		Reset();
		while (currentCircuitGO) yield return null;
		
		if (currentCircuitIndex >= circuitsPrefabs.Length)
			EventManager.Instance.Raise(new CreditsButtonClickedEvent());
		else InstantiateCircuit();
	}
	#endregion

	#region Callbacks to GameManager events
	protected override void GameMenu(GameMenuEvent e)
	{
		Reset();
	}
	protected override void GamePlay(GamePlayEvent e)
	{
		Reset();
	}

	public void GoToNextCircuit(GoToNextCircuitEvent e)
	{
		if (e.eCircuitIndex != -1)
			currentCircuitIndex = e.eCircuitIndex;
		else
			currentCircuitIndex++;

		StartCoroutine(GoToNextCircuitCoroutine());
	}
	#endregion
}
