// using UnityEngine;

// public class CloseDoorState : NPCState
// {
//     float timer = 2f;

//     public CloseDoorState(NPCStateMachine fsm, BaseNPC npc) : base(fsm, npc) { }

//     public override void Enter()
//     {
//         if (npc.animator != null)
//             npc.animator.PlayCloseDoor();
//         timer = 2f;
//     }

//     public override void Update()
//     {
//         timer -= Time.deltaTime;
//         if (timer <= 0)
//             fsm.ChangeState(new SitState(fsm, npc));
//     }
// }
