using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSearchState : PlayerState
{
    public PlayerSearchState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Update()
    {
        base.Update();

        if(!player.isSearching)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

  
}
