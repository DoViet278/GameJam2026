// using UnityEngine;

// public class IdleState : NPCState
// {
//     float timer = 2f;

//     public IdleState(NPCStateMachine fsm, BaseNPC npc) : base(fsm, npc) { }

//     public override void Enter()
//     {
//         timer = 2f;
//         if (npc.animator != null)
//             npc.animator.PlayIdle();
//     }

//     public override void Update()
//     {
//         timer -= Time.deltaTime;
//         if (timer <= 0)
//             fsm.ChangeState(new LookAroundState(fsm, npc));
//     }
// }
