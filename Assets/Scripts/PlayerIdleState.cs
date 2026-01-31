using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(0, rb.velocity.y);

    }

    public override void Update()
    {
        base.Update();

        if(player.moveInput.x != 0 && player.ground2Detected == false && player.groundDetected == false && player.wallDetected == false)
        {
            stateMachine.ChangeState(player.moveState);
        } 

        if(player.moveInput.y != 0 && player.ground2Detected == false && player.groundDetected == false && player.wallDetected == false)
        {
            stateMachine.ChangeState(player.moveUDState);
        }
    }
}
