using UnityEngine;

public class NPCIdleState : NPCState
{
    public NPCIdleState(NPCStateMachine fsm, BaseNPC npc) : base(fsm, npc) { }

    public override void Enter()
    {
        npc.SetIdle();
    }

    public override void Update()
    {
        if (npc.pathMover == null)
            return;

        Vector2 dir = npc.pathMover.Tick(Time.deltaTime);
        if (npc.pathMover.IsMoving)
        {
            npc.UpdateAnimator(dir);
            fsm.ChangeState(new NPCPathPatrolState(fsm, npc));
        }
    }
}
