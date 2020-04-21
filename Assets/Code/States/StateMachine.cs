using SoftLiu.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private string m_lastStateName = "";

    private bool m_isStateTransitioning = false;
    public bool isStateTransitioning
    {
        get { return m_isStateTransitioning; }
        set { m_isStateTransitioning = value; }
    }

    #region States

    protected Dictionary<string, State> m_states = new Dictionary<string, State>();

    // State stack
    protected Stack<State> m_stack = new Stack<State>();

    public State GetStateByName(string name)
    {
        State state = null;
        if (!m_states.TryGetValue(name, out state))
            return null;
        return state;
    }

    public bool IsState<T>() where T : State
    {
        return m_stack.Peek() is T;
    }

    public void Push(string name, ScreenState.EntryBehaviour entryBehaviour, params object[] args)
    {

    }

    #endregion
}
