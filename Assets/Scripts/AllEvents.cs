using UnityEngine;

#region GameManager Events

public class GameMenuEvent : SDD.Events.Event
{
}

public class GamePlayEvent : SDD.Events.Event
{
}

public class GamePauseEvent : SDD.Events.Event
{
}

public class GameResumeEvent : SDD.Events.Event
{
}

public class GameOverEvent : SDD.Events.Event
{
}

public class GameCreditsEvent : SDD.Events.Event
{
}

public class GameVictoryEvent : SDD.Events.Event
{
    public Player ePlayer;
}

public class GameExitEvent : SDD.Events.Event
{
}

public class GameStatisticsChangedEvent : SDD.Events.Event
{
    public int eBestScore { get; set; }
    public int ePlayerNumber { get; set; }
    public int eScore { get; set; }
    public int eNMonstersLeft{ get; set; }
}

#endregion

#region MenuManager Events
public class EscapeButtonClickedEvent : SDD.Events.Event
{
}

public class ResumeButtonClickedEvent : SDD.Events.Event
{
}

public class MainMenuButtonClickedEvent : SDD.Events.Event
{
}

public class PlayButtonClickedEvent : SDD.Events.Event
{
}

public class NextCircuitButtonClickedEvent : SDD.Events.Event
{
}

public class CircuitOneButtonClickedEvent : SDD.Events.Event
{
}

public class CircuitTwoButtonClickedEvent : SDD.Events.Event
{
}

public class CircuitThreeButtonClickedEvent : SDD.Events.Event
{
}

public class CreditsButtonClickedEvent : SDD.Events.Event
{
}

public class ExitButtonClickedEvent : SDD.Events.Event
{
}
#endregion

#region Element Event
public class ElementMustBeDestroyedEvent : SDD.Events.Event
{
    public GameObject eElement;
}

public class ElementIsBeingDestroyedEvent : SDD.Events.Event
{
    public GameObject eElement;
}
#endregion

#region Score Event

public class ScoreItemEvent : SDD.Events.Event
{
    public GameObject eElement;
    public Player ePlayer;
}

#endregion

#region Game Manager Additional Event
public class GoToNextCircuitEvent : SDD.Events.Event
{
    public int eCircuitIndex;
}

public class CircuitButtonClickedEvent : SDD.Events.Event
{
}

public class GameHasStartedEvent : SDD.Events.Event
{
}

public class TimeIsUpEvent : SDD.Events.Event
{
}
#endregion


#region Level Events
public class CircuitHasBeenDestroyedEvent : SDD.Events.Event
{
}

public class CircuitHasBeenInstantiatedEvent : SDD.Events.Event
{
    public Circuit ECircuit;
}
#endregion