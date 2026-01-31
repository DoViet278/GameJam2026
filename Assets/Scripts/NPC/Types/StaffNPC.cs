using UnityEngine;

public class StaffNPC : BaseNPC
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
        Debug.Log("Staff Alert!");
    }
}
