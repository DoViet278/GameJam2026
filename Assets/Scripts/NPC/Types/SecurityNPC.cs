using UnityEngine;

public class SecurityNPC : BaseNPC
{
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        ChangeState(new NPCPathPatrolState(fsm, this));
    }

    public override void Alert()
    {
        Debug.Log("Security Alert!");
    }
}
