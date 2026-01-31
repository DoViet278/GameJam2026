using UnityEngine;

public class NPCMoveState : NPCState
{
    public NPCMoveState(NPCStateMachine fsm, BaseNPC npc) : base(fsm, npc)
    {
    }

    public override void Update()
    {
        if (npc.pathMover == null)
        {
            npc.SetIdle();
            return;
        }

        Vector2 dir = npc.pathMover.Tick(Time.deltaTime);
        if (npc.pathMover.IsMoving)
        {
            npc.UpdateAnimator(dir);
            return;
        }

        npc.SetIdle();
        fsm.ChangeState(new NPCIdleState(fsm, npc));
    }
}
