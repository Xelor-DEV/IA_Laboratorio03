using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : Human
{
    private void Awake()
    {
        typestate = TypeState.Dormir;
        LocadComponent();
    }
    public override void LocadComponent()
    {
        base.LocadComponent();

    }
    public override void Enter()
    {

    }
    public override void Execute()
    {
         

        base.Execute();
    }
    public override void Exit()
    {

    }
}
