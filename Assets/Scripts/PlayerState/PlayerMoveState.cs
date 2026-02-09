using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Update()
    {
        base.Update();

        if(player.moveInput.x == 0 || player.wallDetected == true)
        {
            stateMachine.ChangeState(player.idleState);
        }


        player.SetVelocity(player.moveInput.x * player.moveSpeed , player.rb.velocity.y);
    }


}
