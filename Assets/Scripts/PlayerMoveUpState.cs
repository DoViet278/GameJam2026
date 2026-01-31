using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveUpState : PlayerState
{
    public PlayerMoveUpState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput.y == 0 || player.ground2Detected == true)
        {
            stateMachine.ChangeState(player.idleState);
        }
        if (rb.velocity.y < 0)
        {
            stateMachine.ChangeState(player.moveUDState);
        }

        player.SetVelocity(0, player.moveInput.y * player.moveSpeed);
    }
}
