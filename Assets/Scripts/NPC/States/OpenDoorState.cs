// using UnityEngine;

// public class OpenDoorState : NPCState
// {
//     float timer = 2f;

//     public OpenDoorState(NPCStateMachine fsm, BaseNPC npc) : base(fsm, npc) { }

//     public override void Enter()
//     {
//         if (npc.animator != null)
//             npc.animator.PlayOpenDoor();
//         timer = 2f;
//     }

//     public override void Update()
//     {
//         timer -= Time.deltaTime;
//         if (timer <= 0)
//             fsm.ChangeState(new PatrolState(fsm, npc));
//     }
// }
