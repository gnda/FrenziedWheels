using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using System.Linq;
using DefaultNamespace;

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
            Racers.Sort((r1,r2)=>r1.CurrentDistance.CompareTo(r2.CurrentDistance));
            Timer += Time.deltaTime;
        }
    }

    #endregion

    #region Game State, Mode and Types
    private GameState gameState;
    public int NumberOfCars { get; set; } = 1;

    public bool IsPlaying
    {
        get { return gameState == GameState.gamePlay; }
    }
    #endregion

    #region Elements Instances
    public Transform[] PlayerTransforms
    {
        get { return Players.Select(item => item.transform).ToArray(); }
    }
    
    public List<Player> Players
    {
        get { return FindObjectsOfType<Player>().ToList(); }
    }

    public List<Racer> Racers
    {
        get { return FindObjectsOfType<Racer>().ToList(); }
    }

    public Circuit CurrentCircuit => FindObjectOfType<Circuit>();
    
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

    #endregion
    
    
    //Callbacks to events
    
    
    #region Callbacks to events issued by LevelManager

    private void CircuitHasBeenInstantiated(CircuitHasBeenInstantiatedEvent e)
    {
        Timer = 0;

        EventManager.Instance.Raise(new GameHasStartedEvent());

        SetTimeScale(1);
        gameState = GameState.gamePlay;
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
        NumberOfCars = int.Parse(MenuManager.Instance.TxtNumberOfCars.text);
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