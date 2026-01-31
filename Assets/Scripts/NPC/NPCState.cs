using UnityEngine;

public abstract class NPCState
{
    protected NPCStateMachine fsm;
    protected BaseNPC npc;

    protected NPCState(NPCStateMachine fsm, BaseNPC npc)
    {
        this.fsm = fsm;
        this.npc = npc;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}
