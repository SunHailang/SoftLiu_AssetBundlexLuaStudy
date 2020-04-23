using SoftLiu.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontEndState : ScreenState
{
    public override void OnEnter(State previousState, params object[] args)
    {
        base.OnEnter(previousState, args);
    }

    public override void OnExit(State nextState, ExitBehaviour exitBehaviour)
    {
        base.OnExit(nextState, exitBehaviour);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnHardwareBackButton()
    {
        
    }
}
