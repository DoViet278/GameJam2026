using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveUDState : PlayerState
{
    public PlayerMoveUDState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if(player.moveInput.y == 0 || player.groundDetected == true)
        {
            stateMachine.ChangeState(player.idleState);
        }

        if(player.moveInput.y > 0)
        {
            stateMachine.ChangeState(player.moveUpState);
        }
        player.SetVelocity(0, player.moveInput.y * player.moveSpeed);
    }
}
