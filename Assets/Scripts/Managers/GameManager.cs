using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using System.Linq;

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

public enum GameMode
{
    local,
    multiplayer
}

public enum PlayerType
{
    human,
    computer
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
            Timer += Time.deltaTime;
        }
    }

    #endregion

    #region Game State, Mode and Types
    private GameState gameState;
    private GameMode gameMode;
    private PlayerType playerType;

    public GameState GameState { get { return gameState; }} 
    public GameMode GameMode { get { return gameMode; }} 
    public PlayerType PlayerType { get { return playerType; }}
    public int NumberOfPlayer { get; set; } = 0;

    public bool IsPlaying
    {
        get { return gameState == GameState.gamePlay; }
    }
    #endregion

    #region Score

    public int BestScore
    {
        get { return PlayerPrefs.GetInt("BEST_SCORE", 0); }
        set { PlayerPrefs.SetInt("BEST_SCORE", value); }
    }

    void IncScore(Player player, int score)
    {
        SetScore(player, player.GainedScore + score);
    }

    void SetScore(Player player, int score)
    {
        player.GainedScore = score;

        EventManager.Instance.Raise(new GameStatisticsChangedEvent()
        {eBestScore = BestScore, ePlayerNumber = player.PlayerNumber, 
            eScore = score, eNMonstersLeft = nMonstersLeft});
    }

    #endregion

    #region Monsters to be destroyed
    private int nMonstersLeft;

    void DecrementNMonstersLeft(int decrement)
    {
        SetNMonstersLeft(nMonstersLeft - decrement);
    }

    void SetNMonstersLeft(int nMonsters)
    {
        nMonstersLeft = nMonsters;
        
        EventManager.Instance.Raise(new GameStatisticsChangedEvent()
        { eNMonstersLeft = nMonstersLeft });
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
        EventManager.Instance.AddListener<CircuitOneButtonClickedEvent>(CircuitOneButtonClicked);
        EventManager.Instance.AddListener<CircuitTwoButtonClickedEvent>(CircuitTwoButtonClicked);
        EventManager.Instance.AddListener<CircuitThreeButtonClickedEvent>(CircuitThreeButtonClicked);
        
        //Circuit Generated
        EventManager.Instance.AddListener<CircuitHasBeenInstantiatedEvent>(CircuitHasBeenInstantiated);

        //Score Item
        EventManager.Instance.AddListener<ScoreItemEvent>(ScoreHasBeenGained);
    }

    public override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();

        //MainMenuManager
        EventManager.Instance.RemoveListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
        EventManager.Instance.RemoveListener<NextCircuitButtonClickedEvent>(NextCircuitButtonClicked);
        EventManager.Instance.RemoveListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
        EventManager.Instance.RemoveListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
        
        //Exit
        EventManager.Instance.AddListener<ExitButtonClickedEvent>(ExitButtonClicked);

        //Circuit Select
        EventManager.Instance.RemoveListener<CircuitOneButtonClickedEvent>(CircuitOneButtonClicked);
        EventManager.Instance.RemoveListener<CircuitTwoButtonClickedEvent>(CircuitTwoButtonClicked);
        EventManager.Instance.RemoveListener<CircuitThreeButtonClickedEvent>(CircuitThreeButtonClicked);
        
        //Circuit Generated
        EventManager.Instance.RemoveListener<CircuitHasBeenInstantiatedEvent>(CircuitHasBeenInstantiated);

        //Score Item
        EventManager.Instance.RemoveListener<ScoreItemEvent>(ScoreHasBeenGained);
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
    
    
    #region Callbacks to events issued by GameManager
    
    private void TimeIsUp(TimeIsUpEvent e)
    {
        StartCoroutine(CheckVictory());
    }
    
    #endregion

    #region Callbacks to events issued by LevelManager

    private void CircuitHasBeenInstantiated(CircuitHasBeenInstantiatedEvent e)
    {
        Timer = 0;
        //nMonstersLeft = Monsters.Count;
        
        EventManager.Instance.Raise(new GameHasStartedEvent());
        /*EventManager.Instance.Raise(new GameStatisticsChangedEvent() 
            { eBestScore = BestScore, ePlayerNumber = -1, 
                eNMonstersLeft = Monsters.Count});*/
        
        SetTimeScale(1);
        gameState = GameState.gamePlay;
    }

    #endregion

    #region Callbacks to events issued by Score items

    private void ScoreHasBeenGained(ScoreItemEvent e)
    {
        IScore elementWithScore = e.eElement.GetComponent<IScore>();
        //IMoveable moveableElement = e.eElement.GetComponent<IMoveable>();

        /*if ((moveableElement != null && elementWithScore != null) &&
            !moveableElement.IsDestroyed)
            IncScore(e.ePlayer, elementWithScore.Score);*/
    }

    private IEnumerator CheckVictory()
    {
        if (Players.Count > 0)
        {
            Players.Sort((a,b) => a.GainedScore - b.GainedScore);

            if (Players[0].GainedScore != 0)
            {
                // Delay to Check if both last player and last enemy are dead
                yield return new WaitForSeconds(0.1f);

                /*if (Players.Count > 0 && Monsters.Count == 0) 
                    Victory(Players[0]);*/
            } else Over();
        }else Over();
    }

    #endregion

    #region Callbacks to events issued by Element

    #endregion

    #region Callbacks to events issued by Circuit

    #endregion

    
    // Callbacks to MenuManager UI events
    
    
    #region Callbacks to General UI Events
    private void EscapeButtonClicked(EscapeButtonClickedEvent e)
    {
        if (IsPlaying) Pause();
    }
    
    private void MainMenuButtonClicked(MainMenuButtonClickedEvent e)
    {
        EventManager.Instance.Raise(new GameStatisticsChangedEvent() 
            {eBestScore = BestScore, ePlayerNumber = -1});
        Menu();
    }

    private void NextCircuitButtonClicked(NextCircuitButtonClickedEvent e)
    {
        EventManager.Instance.Raise(new GameStatisticsChangedEvent() 
            {eBestScore = BestScore, ePlayerNumber = -1});
        EventManager.Instance.Raise(new GoToNextCircuitEvent()
            {eCircuitIndex = -1});
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

    private void Over()
    {
        SetTimeScale(0);
        gameState = GameState.gameOver;
        SfxManager.Instance.PlaySfx(Constants.GAMEOVER_SFX);
        EventManager.Instance.Raise(new GameOverEvent());
    }

    private void Victory(Player player)
    {
        if (player.GainedScore > BestScore)
            BestScore = player.GainedScore;
        
        SetTimeScale(0);
        gameState = GameState.gameVictory;
        SfxManager.Instance.PlaySfx(Constants.VICTORY_SFX);
        EventManager.Instance.Raise(new GameVictoryEvent() {ePlayer = player});
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