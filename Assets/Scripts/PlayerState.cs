using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : EntityState
{
    protected PlayerController player;
    protected PlayerInputSet inputActions;
    protected PlayerState(PlayerController player ,PlayerStateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;

        rb = player.rb;
        anim = player.anim;
        inputActions = player.input;
    }

    public override void Update()
    {
        base.Update();
    }

}
