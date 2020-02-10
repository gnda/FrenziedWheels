using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using System.Linq;
using DefaultNamespace;
using UnityEngine.Experimental.GlobalIllumination;
using UnityStandardAssets.Utility;
using UnityStandardAssets.Vehicles.Car;

#region Game states, mode and types
public enum GameState
{
    gameMenu,
    gamePlay,
    gameNextLevel,
    gamePause,
    gameOver,
    gameVictory,
    gameCredits
}
#endregion

public class GameManager : Manager<GameManager>
{
    
    [Header(("GeneralPrefabs"))]
    [SerializeField] public GameObject[] CarPrefabs;
    [SerializeField] public GameObject[] AiCarPrefabs;
    [SerializeField] public GameObject mainLight;
    
    #region Time

    void SetTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }

    public float Timer { get; set; } = 0;

    private void FixedUpdate()
    {
        if (IsPlaying)
        {
            Timer += Time.deltaTime;
        }
    }

    #endregion

    #region Game State, Mode and Types
    private GameState gameState;
    public int NumberOfCars { get; set; } = 1;
    public int playerCarIndex = 0;

    public bool IsPlaying
    {
        get { return gameState == GameState.gamePlay; }
    }
    #endregion

    #region Elements Instances
    public List<Player> Players
    {
        get { return FindObjectsOfType<Player>().ToList(); }
    }

    public List<Racer> Racers
    {
        get { return FindObjectsOfType<Racer>().ToList(); }
    }
    
    public int NumberOfCarModels
    {
        get
        {
            int cpt = 0;
            foreach (GameObject carPrefab in CarPrefabs)
            {
                foreach (Car car in carPrefab.GetComponents<Car>())
                {
                    cpt += car.Palettes.Length;
                }
            }

            return cpt;
        }
    }

    public GameObject GetSelectedCar()
    {
        playerCarIndex = playerCarIndex >= 0 ? playerCarIndex % NumberOfCarModels : 
            NumberOfCarModels + playerCarIndex;
        int cpt = 0;

        for (int i = 0; i < CarPrefabs.Length; i++)
        {
            Material[] Palettes = CarPrefabs[i].GetComponent<Car>().Palettes;
            
            for (int j = 0; j < Palettes.Length; j++)
            {
                if (playerCarIndex == cpt + j)
                {
                    GameObject car = CarPrefabs[i];
                    car.transform.Find("Car").Find("CarBody").
                        GetComponent<Renderer>().material = Palettes[j];
                    
                    return car;
                }
            }

            cpt += Palettes.Length;
        }

        // Car not found
        return new GameObject();
    }

    public void DisableCarComponents(GameObject car)
    {
        car.GetComponent<Rigidbody>().isKinematic = true;
        car.GetComponent<Car>().enabled = false;
        car.GetComponent<CarAudio>().enabled = false;
        car.GetComponent<CarUserControl>().enabled = false;
        car.transform.Find("Particles").gameObject.SetActive(false);
        car.transform.GetComponentsInChildren<WheelEffects>().
            ToList().ForEach(w => w.enabled = false);
    }

    public Circuit CurrentCircuit => FindObjectOfType<Circuit>();
    
    public List<Transform> CurrentCircuitWaypoints => 
        CurrentCircuit.GetComponentInChildren<WaypointCircuit>().Waypoints.ToList();
    
    #endregion

    #region Events' subscription

    public override void SubscribeEvents()
    {
        base.SubscribeEvents();

        //MainMenuManager
        EventManager.Instance.AddListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
        EventManager.Instance.AddListener<NextCircuitButtonClickedEvent>(NextCircuitButtonClicked);
        EventManager.Instance.AddListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
        EventManager.Instance.AddListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
        EventManager.Instance.AddListener<CreditsButtonClickedEvent>(CreditsButtonClicked);
        
        //Exit
        EventManager.Instance.AddListener<ExitButtonClickedEvent>(ExitButtonClicked);

        //Circuit Select
        EventManager.Instance.AddListener<CircuitButtonClickedEvent>(CircuitButtonClicked);
        EventManager.Instance.AddListener<CircuitOneButtonClickedEvent>(CircuitOneButtonClicked);
        EventManager.Instance.AddListener<CircuitTwoButtonClickedEvent>(CircuitTwoButtonClicked);
        EventManager.Instance.AddListener<CircuitThreeButtonClickedEvent>(CircuitThreeButtonClicked);
        EventManager.Instance.AddListener<CircuitFourButtonClickedEvent>(CircuitFourButtonClicked);
        EventManager.Instance.AddListener<CircuitFiveButtonClickedEvent>(CircuitFiveButtonClicked);
        EventManager.Instance.AddListener<CircuitSixButtonClickedEvent>(CircuitSixButtonClicked);
        EventManager.Instance.AddListener<CircuitSevenButtonClickedEvent>(CircuitSevenButtonClicked);
        EventManager.Instance.AddListener<CircuitEightButtonClickedEvent>(CircuitEightButtonClicked);
        EventManager.Instance.AddListener<CircuitNineButtonClickedEvent>(CircuitNineButtonClicked);
        
        //Circuit Generated
        EventManager.Instance.AddListener<CircuitHasBeenInstantiatedEvent>(CircuitHasBeenInstantiated);
    }

    public override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();

        //MainMenuManager
        EventManager.Instance.RemoveListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
        EventManager.Instance.RemoveListener<NextCircuitButtonClickedEvent>(NextCircuitButtonClicked);
        EventManager.Instance.RemoveListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
        EventManager.Instance.RemoveListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
        EventManager.Instance.RemoveListener<CreditsButtonClickedEvent>(CreditsButtonClicked);
        
        //Exit
        EventManager.Instance.RemoveListener<ExitButtonClickedEvent>(ExitButtonClicked);

        //Circuit Select
        EventManager.Instance.RemoveListener<CircuitButtonClickedEvent>(CircuitButtonClicked);
        EventManager.Instance.RemoveListener<CircuitOneButtonClickedEvent>(CircuitOneButtonClicked);
        EventManager.Instance.RemoveListener<CircuitTwoButtonClickedEvent>(CircuitTwoButtonClicked);
        EventManager.Instance.RemoveListener<CircuitThreeButtonClickedEvent>(CircuitThreeButtonClicked);
        EventManager.Instance.RemoveListener<CircuitFourButtonClickedEvent>(CircuitFourButtonClicked);
        EventManager.Instance.RemoveListener<CircuitFiveButtonClickedEvent>(CircuitFiveButtonClicked);
        EventManager.Instance.RemoveListener<CircuitSixButtonClickedEvent>(CircuitSixButtonClicked);
        EventManager.Instance.RemoveListener<CircuitSevenButtonClickedEvent>(CircuitSevenButtonClicked);
        EventManager.Instance.RemoveListener<CircuitEightButtonClickedEvent>(CircuitEightButtonClicked);
        EventManager.Instance.RemoveListener<CircuitNineButtonClickedEvent>(CircuitNineButtonClicked);
        
        //Circuit Generated
        EventManager.Instance.RemoveListener<CircuitHasBeenInstantiatedEvent>(CircuitHasBeenInstantiated);
    }
    
    #endregion

    #region Manager implementation

    protected override IEnumerator InitCoroutine()
    {
        Menu();

        yield break;
    }
    
    protected IEnumerator WaitCountdownCoroutine()
    {
        EventManager.Instance.Raise(new DisplayCountdownEvent());
        Racers.ForEach(r =>
        {
            if (r.GetComponent<CarUserControl>())
            {
                r.GetComponent<CarUserControl>().enabled = false;
            }
            if (r.GetComponent<CarAIControl>())
            {
                r.GetComponent<CarAIControl>().enabled = false;
            }
        });
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(StartRaceCoroutine());
    }

    protected IEnumerator StartRaceCoroutine()
    {
        gameState = GameState.gamePlay;
        Racers.ForEach(r =>
        {
            if (r.GetComponent<CarUserControl>())
            {
                r.GetComponent<CarUserControl>().enabled = true;
            }
            if (r.GetComponent<CarAIControl>())
            {
                r.GetComponent<CarAIControl>().enabled = true;
            }
        });
        EventManager.Instance.Raise(new GameHasStartedEvent());
        yield return null;
    }
    #endregion
    
    
    //Callbacks to events
    
    
    #region Callbacks to events issued by LevelManager

    private void CircuitHasBeenInstantiated(CircuitHasBeenInstantiatedEvent e)
    {
        Timer = 0;
        StartCoroutine(WaitCountdownCoroutine());
        SetTimeScale(1);
    }

    #endregion
    

    // Callbacks to MenuManager UI events
    
    
    #region Callbacks to General UI Events
    private void EscapeButtonClicked(EscapeButtonClickedEvent e)
    {
        if (IsPlaying) Pause();
    }
    
    private void MainMenuButtonClicked(MainMenuButtonClickedEvent e)
    {
        Menu();
    }

    private void CircuitButtonClicked(CircuitButtonClickedEvent e)
    {
        NumberOfCars = MenuManager.Instance.NumberOfCars;
        mainLight.SetActive(false);
    }

    private void NextCircuitButtonClicked(NextCircuitButtonClickedEvent e)
    {
        EventManager.Instance.Raise(new GoToNextCircuitEvent()
        {
            eCircuitIndex = -1
        });
    }

    private void ResumeButtonClicked(ResumeButtonClickedEvent e)
    {
        Resume();
    }

    private void CreditsButtonClicked(CreditsButtonClickedEvent e)
    {
        Credits();
    }

    private void ExitButtonClicked(ExitButtonClickedEvent e)
    {
        Exit();
    }
    #endregion

    #region Callbacks to Level UI Events
    private void CircuitOneButtonClicked(CircuitOneButtonClickedEvent e)
    {
        Play(1);
    }

    private void CircuitTwoButtonClicked(CircuitTwoButtonClickedEvent e)
    {
        Play(2);
    }

    private void CircuitThreeButtonClicked(CircuitThreeButtonClickedEvent e)
    {
        Play(3);
    }

    private void CircuitFourButtonClicked(CircuitFourButtonClickedEvent e)
    {
        Play(4);
    }

    private void CircuitFiveButtonClicked(CircuitFiveButtonClickedEvent e)
    {
        Play(5);
    }

    private void CircuitSixButtonClicked(CircuitSixButtonClickedEvent e)
    {
        Play(6);
    }

    private void CircuitSevenButtonClicked(CircuitSevenButtonClickedEvent e)
    {
        Play(7);
    }

    private void CircuitEightButtonClicked(CircuitEightButtonClickedEvent e)
    {
        Play(8);
    }
    
    private void CircuitNineButtonClicked(CircuitNineButtonClickedEvent e)
    {
        Play(9);
    }
    #endregion
    
    
    //Methods linked to callbacks
    

    #region Game flow methods
    void InitNewGame(int levelNumber)
    {
        gameState = GameState.gameNextLevel;
        EventManager.Instance.Raise(new GoToNextCircuitEvent()
            {eCircuitIndex = --levelNumber});
    }
    
    private void Menu()
    {
        SetTimeScale(1);
        gameState = GameState.gameMenu;
        mainLight.SetActive(true);
        MusicLoopsManager.Instance.PlayMusic(Constants.MENU_MUSIC);
        EventManager.Instance.Raise(new GameMenuEvent());
    }

    private void Play(int levelNumber)
    {
        EventManager.Instance.Raise(new GamePlayEvent());
        InitNewGame(levelNumber);
    }

    private void Pause()
    {
        SetTimeScale(0);
        gameState = GameState.gamePause;
        EventManager.Instance.Raise(new GamePauseEvent());
    }

    private void Resume()
    {
        SetTimeScale(1);
        gameState = GameState.gamePlay;
        EventManager.Instance.Raise(new GameResumeEvent());
    }

    protected override void GameOver(GameOverEvent e)
    {
        SetTimeScale(0);
        gameState = GameState.gameOver;
        SfxManager.Instance.PlaySfx(Constants.GAMEOVER_SFX);
    }

    protected override void GameVictory(GameVictoryEvent e)
    {
        SetTimeScale(0);
        gameState = GameState.gameVictory;
        SfxManager.Instance.PlaySfx(Constants.VICTORY_SFX);
    }

    private void Credits()
    {
        SetTimeScale(1);
        gameState = GameState.gameCredits;
        MusicLoopsManager.Instance.PlayMusic(Constants.MENU_MUSIC);
        EventManager.Instance.Raise(new GameCreditsEvent());
    }

    private void Exit()
    {
        SetTimeScale(0);
        Application.Quit();
    }
    #endregion
}