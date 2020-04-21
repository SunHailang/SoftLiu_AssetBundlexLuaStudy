using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.State
{
    public class State
    {
        public enum ExitBehaviour
        {
            None = 0,
            Disable = 1,
            DisableInput = 2,
            Destroy = 3,
        }

        private string m_name = string.Empty;
        public string name { get { return m_name; } }

        private float m_timeInState = 0f;
        private float m_totalTimeInState = 0f;


        #region Interface

        public virtual void OnCreate(params object[] args)
        {
            Debug.Log("State OnCreate name: " + name);
        }
        public virtual void OnEnter(State previousState, params object[] args)
        {
            Debug.Log(string.Format("State OnEnter name: {0} , previous State: ", name, previousState.name));
            m_totalTimeInState += m_timeInState;
            m_timeInState = 0f;
        }
        public virtual void OnExit(State nextState, ExitBehaviour exitBehaviour)
        {
            Debug.Log(string.Format("State OnExit name: {0} , next State: ", name, nextState.name));
        }
        public virtual void OnResume(State previousState, params object[] args)
        {
            Debug.Log(string.Format("State OnResume name: {0} , previous State: ", name, previousState.name));
            m_totalTimeInState += m_timeInState;
            m_timeInState = 0f;
        }
        public virtual void OnPause(State nextState, ExitBehaviour exitBehaviour)
        {
            Debug.Log(string.Format("State OnPause name: {0} , next State: ", name, nextState.name));
        }

        public virtual void OnUpdate()
        {
            m_timeInState += Time.unscaledDeltaTime;
        }
        public virtual void OnHardwareBackButton()
        {
            Debug.Log(string.Format("State OnHardwareBackButton name: {0}", name));
        }
        public virtual void OnFixedUpdate()
        {

        }
        public virtual void OnLateUpdate()
        {

        }
        public virtual void OnApplicationPause(bool paused)
        {

        }

        protected virtual void OnStateLoaded()
        {

        }

        public float GetTimeInState()
        {
            return m_timeInState;
        }
        public float GetTotalTimeInState()
        {
            return m_totalTimeInState + m_timeInState;
        }
        #endregion
    }
}
