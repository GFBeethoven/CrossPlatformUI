using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void Initialize(FSM fsm);

    public void Enter(StateEnterData enterData);

    public void Exit();

    public void Update();

    public void FixedUpdate();
}
