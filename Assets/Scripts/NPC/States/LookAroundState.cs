// using UnityEngine;

// public class LookAroundState : NPCState
// {
//     float timer = 3f;
//     bool lookLeft = true;

//     public LookAroundState(NPCStateMachine fsm, BaseNPC npc) : base(fsm, npc) { }

//     public override void Enter()
//     {
//         timer = 3f;
//     }

//     public override void Update()
//     {
//         timer -= Time.deltaTime;

//         if (lookLeft)
//             npc.transform.rotation = Quaternion.Euler(0, 180, 0);
//         else
//             npc.transform.rotation = Quaternion.identity;

//         lookLeft = !lookLeft;

//         if (timer <= 0)
//             fsm.ChangeState(new IdleState(fsm, npc));
//     }
// }
