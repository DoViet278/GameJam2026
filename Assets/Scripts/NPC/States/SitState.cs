// using UnityEngine;

// public class SitState : NPCState
// {
//     float timer = 5f;

//     public SitState(NPCStateMachine fsm, BaseNPC npc) : base(fsm, npc) { }

//     public override void Enter()
//     {
//         if (npc.animator != null)
//             npc.animator.PlaySit();
//         timer = 5f;
//     }

//     public override void Update()
//     {
//         timer -= Time.deltaTime;
//         if (timer <= 0)
//             fsm.ChangeState(new OpenDoorState(fsm, npc));
//     }
// }
