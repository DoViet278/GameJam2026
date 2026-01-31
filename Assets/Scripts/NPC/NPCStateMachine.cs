public class NPCStateMachine
{
    public NPCState CurrentState { get; private set; }

    public void ChangeState(NPCState newState)
    {
        if (CurrentState == newState)
            return;

        if (CurrentState != null)
            CurrentState.Exit();

        CurrentState = newState;

        if (CurrentState != null)
            CurrentState.Enter();
    }

    public void Update()
    {
        if (CurrentState != null)
            CurrentState.Update();
    }
}
