using UnityEngine;

public class Player : MonoBehaviour, IScore
{
    #region Settings & Variables
    [Header("PlayerSettings")]
    [SerializeField] private int score = 0;
    public GameObject bombPrefab;
    
    private static int playerCount = 0;
    private int playerNumber = 0;
    private int gainedScore;
    #endregion
    
    #region Player methods
    public static int PlayerCount 
    {
        get => playerCount;
        set => playerCount = value;
    }

    public int PlayerNumber
    {
        get => playerNumber;
        set => playerNumber = value;
    }
    #endregion
    
    #region Score
    public int Score
    {
        get { return score; }
    }
    
    public int GainedScore
    {
        get => gainedScore;
        set => gainedScore = value;
    }
    #endregion
    
    #region MonoBehaviour lifecycle
    private void Start()
    {
        playerCount++;
        playerNumber = playerCount;
    }
    #endregion
}