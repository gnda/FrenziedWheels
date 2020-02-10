using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SDD.Events;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Vehicles.Car;

public class MenuManager : Manager<MenuManager>
{
    #region Panels
    [Header("Panels")]
    [SerializeField] GameObject panelMainMenu;
    [SerializeField] GameObject panelCarSelection;
    [SerializeField] GameObject panelCircuitSelection;
    [SerializeField] GameObject panelInGameMenu;
    [SerializeField] GameObject panelVictory;
    [SerializeField] GameObject panelGameOver;
    [SerializeField] GameObject panelCredits;
    [SerializeField] GameObject panelCountdown;
    
    [Header("Fields")]
    [SerializeField] Text txtVictoryPlayer;
    [SerializeField] Slider sliderNumberOfCars;
    [SerializeField] Text txtCarAmount;

    public int NumberOfCars => (int) sliderNumberOfCars.value;

    [Header("Settings")]
    [SerializeField] float creditsDuration;
    [SerializeField] private Transform carModelPosition;

    List<GameObject> allPanels;
    #endregion
    
    #region Modals
    List<GameObject> allModals;
    #endregion

    #region Events' subscription
    public override void SubscribeEvents()
    {
        base.SubscribeEvents();
        
        //GameManager
        EventManager.Instance.AddListener<GoToNextCircuitEvent>(GoToNextCircuit);
        EventManager.Instance.AddListener<DisplayCountdownEvent>(DisplayCountdown);

        //Car selection
        EventManager.Instance.AddListener<PlayButtonClickedEvent>(PlayButtonClicked);

        //Circuit selection
        EventManager.Instance.AddListener<CircuitButtonClickedEvent>(CircuitButtonClicked);
    }

    public override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        
        //GameManager
        EventManager.Instance.RemoveListener<GoToNextCircuitEvent>(GoToNextCircuit);
        EventManager.Instance.RemoveListener<DisplayCountdownEvent>(DisplayCountdown);

        //Car selection
        EventManager.Instance.RemoveListener<PlayButtonClickedEvent>(PlayButtonClicked);

        //Circuit selection
        EventManager.Instance.RemoveListener<CircuitButtonClickedEvent>(CircuitButtonClicked);
    }
    #endregion

    #region Manager implementation
    protected override IEnumerator InitCoroutine()
    {
        yield break;
    }
    #endregion

    #region MonoBehaviour lifecycle

    protected override void Awake()
    {
        base.Awake();
        RegisterPanels();
        RegisterModals();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            EscapeButtonHasBeenClicked();
        }
    }

    #endregion

    #region Additional coroutines
    private IEnumerator GoBackToMainMenuCoroutine(float time)
    {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        EventManager.Instance.Raise(new GameMenuEvent());
    }
    #endregion
    
    #region Panel Methods
    void RegisterPanels()
    {
        allPanels = new List<GameObject>();
        allPanels.Add(panelMainMenu);
        allPanels.Add(panelCarSelection);
        allPanels.Add(panelCircuitSelection);
        allPanels.Add(panelInGameMenu);
        allPanels.Add(panelVictory);
        allPanels.Add(panelGameOver);
        allPanels.Add(panelCredits);
        allPanels.Add(panelCountdown);
    }

    void OpenPanel(GameObject panel)
    {
        foreach (var item in allPanels)
            if (item)
                item.SetActive(item == panel);
    }
    #endregion
    
    #region Modal Methods
    void RegisterModals()
    {
        allModals = new List<GameObject>();
    }

    void OpenModal(GameObject modal)
    {
        foreach (var item in allModals)
            if (item)
                item.SetActive(item == modal);
    }

    void DisplayModelCar()
    {
        foreach (Transform child in carModelPosition) {
            Destroy(child.gameObject);
        }
        GameObject carPrefab = GameManager.Instance.GetSelectedCar();
        GameObject carGO = Instantiate(carPrefab, carModelPosition);
        GameManager.Instance.DisableCarComponents(carGO);
    }
    #endregion

    
    // UI OnClick Events
    

    #region General UI OnClick Events
    public void EscapeButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new EscapeButtonClickedEvent());
    }

    public void MainMenuButtonHasBeenClicked()
    {
        foreach (Transform child in carModelPosition) {
            Destroy(child.gameObject);
        }
        FindObjectOfType<AutoCam>().transform.position = Vector3.zero;
        FindObjectOfType<AutoCam>().transform.rotation = Quaternion.identity;
        EventManager.Instance.Raise(new MainMenuButtonClickedEvent());
    }
    
    public void PreviousCarButtonHasBeenClicked()
    {
        GameManager.Instance.playerCarIndex--;
        DisplayModelCar();
    }
    
    public void NextCarButtonHasBeenClicked()
    {
        GameManager.Instance.playerCarIndex++;
        DisplayModelCar();
    }
    
    public void CarAmountHasBeenChanged()
    {
        txtCarAmount.text = sliderNumberOfCars.value.ToString();
    }

    public void CircuitButtonHasBeenClicked()
    {
        FindObjectOfType<AutoCam>().enabled = true;
        carModelPosition.gameObject.SetActive(false);
        foreach (Transform child in carModelPosition) {
            Destroy(child.gameObject);
        }
        FindObjectOfType<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        EventManager.Instance.Raise(new CircuitButtonClickedEvent());
    }

    public void PlayButtonHasBeenClicked()
    {
        FindObjectOfType<AutoCam>().enabled = false;
        carModelPosition.gameObject.SetActive(true);
        DisplayModelCar();
        FindObjectOfType<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        FindObjectOfType<Canvas>().planeDistance = 50;
        EventManager.Instance.Raise(new PlayButtonClickedEvent());
    }
    
    public void ResumeButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new ResumeButtonClickedEvent());
    }

    public void NextCircuitButtonHasBeenClicked()
    {
        FindObjectOfType<AutoCam>().transform.position = Vector3.zero;
        FindObjectOfType<AutoCam>().transform.rotation = Quaternion.identity;
        EventManager.Instance.Raise(new NextCircuitButtonClickedEvent());
    }

    public void CreditsButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new CreditsButtonClickedEvent());
    }

    public void ExitButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new ExitButtonClickedEvent());
    }
    #endregion

    #region Circuit UI OnClick Events
    public void CircuitOneButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new CircuitOneButtonClickedEvent());
    }

    public void CircuitTwoButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new CircuitTwoButtonClickedEvent());
    }

    public void CircuitThreeButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new CircuitThreeButtonClickedEvent());
    }
    
    public void CircuitFourButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new CircuitFourButtonClickedEvent());
    }
    
    public void CircuitFiveButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new CircuitFiveButtonClickedEvent());
    }
    
    public void CircuitSixButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new CircuitSixButtonClickedEvent());
    }
    
    public void CircuitSevenButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new CircuitSevenButtonClickedEvent());
    }
    
    public void CircuitEightButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new CircuitEightButtonClickedEvent());
    }
    
    public void CircuitNineButtonHasBeenClicked()
    {
        EventManager.Instance.Raise(new CircuitNineButtonClickedEvent());
    }
    #endregion
    
    
    // Callbacks to MenuManager UI events
    
    
    #region Callbacks to General UI events
    private void PlayButtonClicked(PlayButtonClickedEvent e)
    {
        OpenPanel(panelCarSelection);
    }

    private void CircuitButtonClicked(CircuitButtonClickedEvent e)
    {
        OpenPanel(panelCircuitSelection);
    }
    #endregion

    
    // Callbacks to GameManager UI events
    
    
    protected void DisplayCountdown(DisplayCountdownEvent e)
    {
        OpenPanel(panelCountdown);
    }
    
    
    // Callbacks to GameManager events
    
    
    #region Callbacks to GameManager events
    private void GoToNextCircuit(GoToNextCircuitEvent e)
    {
        OpenPanel(null);
    }

    protected override void GameMenu(GameMenuEvent e)
    {
        OpenPanel(panelMainMenu);
    }

    protected override void GamePlay(GamePlayEvent e)
    {
        OpenPanel(null);
    }

    protected override void GamePause(GamePauseEvent e)
    {
        OpenPanel(panelInGameMenu);
    }

    protected override void GameResume(GameResumeEvent e)
    {
        OpenPanel(null);
    }

    protected override void GameOver(GameOverEvent e)
    {
        OpenPanel(panelGameOver);
    }

    protected override void GameVictory(GameVictoryEvent e)
    {
        txtVictoryPlayer.text = e.ePlayer.PlayerNumber.ToString();
        OpenPanel(panelVictory);
    }

    protected override void GameCredits(GameCreditsEvent e)
    {
        OpenPanel(panelCredits);
        StartCoroutine(GoBackToMainMenuCoroutine(creditsDuration));
    }
    #endregion
}