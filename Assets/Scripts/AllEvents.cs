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
    public int ePosition { get; set; }
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
#endregion


#region Level Events
public class CircuitHasBeenDestroyedEvent : SDD.Events.Event
{
}

public class CircuitHasBeenInstantiatedEvent : SDD.Events.Event
{
    public Circuit eCircuit;
}
#endregion