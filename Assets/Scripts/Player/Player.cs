using UnityEngine;

public class Player : MonoBehaviour
{
    #region Settings & Variables
    [Header("PlayerSettings")]

    private static int playerCount = 0;
    private int playerNumber = 0;
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

    #region MonoBehaviour lifecycle
    private void Start()
    {
        playerCount++;
        playerNumber = playerCount;
    }
    #endregion
}