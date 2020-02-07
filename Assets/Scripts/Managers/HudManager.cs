using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using SDD.Events;
using UnityStandardAssets.Vehicles.Car;

public class HudManager : Manager<HudManager>
{
	#region Labels & Values
	[Header("HudManager")]
	[SerializeField] private GameObject panelHUD;
	
	[Header("Texts")]
	[SerializeField] private Text txtLaps;
	[SerializeField] private Text txtPosition;
	[SerializeField] private Text txtSpeed;
	[SerializeField] private Text txtTime;
	#endregion
	
	#region Monobehaviour lifecycle
	private void Update()
	{
		if (GameManager.Instance.IsPlaying)
		{
			Player player = GameManager.Instance.Players[0];
			txtLaps.text = (player.GetComponent<Racer>().Laps) + "/" +
			    (GameManager.Instance.CurrentCircuit.MaxLaps);
			txtPosition.text = player.GetComponent<Racer>().Position + "/" + 
			                   GameManager.Instance.Racers.Count;
			txtSpeed.text = ((int)player.GetComponent<CarController>().CurrentSpeed).ToString();
			txtTime.text = new DateTime(
			(long)(GameManager.Instance.Timer * TimeSpan.TicksPerSecond))
				.ToString("mm:ss:ff");
		}
	}
	#endregion

	#region Manager implementation
	protected override IEnumerator InitCoroutine()
	{
		panelHUD.SetActive(false);
		yield break;
	}
	#endregion

	#region Events subscription
	public override void SubscribeEvents()
	{
		base.SubscribeEvents();

		//level
		EventManager.Instance.AddListener<GameHasStartedEvent>(GameHasStarted);
		EventManager.Instance.AddListener<GoToNextCircuitEvent>(GoToNextLevel);
		
	}
	public override void UnsubscribeEvents()
	{
		base.UnsubscribeEvents();

		//level
		EventManager.Instance.RemoveListener<GameHasStartedEvent>(GameHasStarted);
		EventManager.Instance.RemoveListener<GoToNextCircuitEvent>(GoToNextLevel);

	}
	#endregion

	#region Callbacks to Level events

	private void GoToNextLevel(GoToNextCircuitEvent e)
	{
		panelHUD.SetActive(true);
	}
	#endregion

	#region Callbacks to GameManager events
	protected override void GameStatisticsChanged(GameStatisticsChangedEvent e)
	{
		/*List<Transform> playersScoreActiveTransform = 
			playersScoreTransforms.FindAll(
				item => item.gameObject.activeInHierarchy);
		
		List<Text> playersScore = new List<Text>();

		foreach (var transform in playersScoreActiveTransform)
			playersScore.Add(transform.GetComponentsInChildren<Text>()[1]);

		if (e.ePlayerNumber != -1)
		{
			if(e.eScore != 0)
				playersScore[e.ePlayerNumber - 1].text = e.eScore.ToString();
		}
		else
			foreach (var score in playersScore)
				score.text = "0";
		
		if (e.eBestScore != 0)
			txtBestScore.text = e.eBestScore.ToString();

        txtMonstersLeft.text = e.eNMonstersLeft.ToString();*/
	}

	private void GameHasStarted(GameHasStartedEvent e)
	{
		txtTime.text = GameManager.Instance.Timer.ToString();
	}
	#endregion
	
	#region Callbacks to MenuManager events
	protected override void GameMenu(GameMenuEvent e)
	{
		if (panelHUD.activeInHierarchy)
		{
			panelHUD.SetActive(false);
		}
	}
	
	protected override void GamePlay(GamePlayEvent e)
	{
		/*int nbPlayer = GameManager.Instance.NumberOfPlayer;
		
		for (int i = nbPlayer; i < 4; i++)
			playersScoreTransforms[i].gameObject.SetActive(false);*/
	}
	
	protected override void GameCredits(GameCreditsEvent e)
	{
		panelHUD.SetActive(false);
	}
	#endregion
}