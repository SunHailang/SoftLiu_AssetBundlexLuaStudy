using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftLiu.State
{
    public class ScreenState : State
    {
        protected string m_scene = string.Empty;

        private ScreenTracker m_tracker;

        public enum EntryBehaviour
        {
            None = 0,
            Load = 1,
            LoadAdditive = 2
        }

        protected bool m_loadedEventSent = false;

        private bool m_isFirstLoad = true;

        public bool isFirstLoad { get { return m_isFirstLoad; } protected set { m_isFirstLoad = value; } }

        public override void OnCreate(params object[] args)
        {
            base.OnCreate(args);
            if (args != null && args.Length > 0)
            {
                m_scene = args[0].ToString();
            }
        }
        public override void OnEnter(State previousState, params object[] args)
        {
            base.OnEnter(previousState, args);

        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (m_tracker != null)
            {

            }
        }
        public override void OnHardwareBackButton()
        {
            base.OnHardwareBackButton();
        }
        public override void OnExit(State nextState, ExitBehaviour exitBehaviour)
        {
            base.OnExit(nextState, exitBehaviour);
        }
        public override void OnResume(State previousState, params object[] args)
        {
            base.OnResume(previousState, args);
            m_loadedEventSent = false;
        }
        public override void OnPause(State nextState, ExitBehaviour exitBehaviour)
        {
            base.OnPause(nextState, exitBehaviour);
        }

        protected void StateLoaded()
        {
            OnStateLoaded();
            m_isFirstLoad = false;
        }
    }
}
