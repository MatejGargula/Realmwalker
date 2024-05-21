using System;
using UnityEngine;

namespace BT
{
    public abstract class Node : ScriptableObject
    {
        #region Enum

        public enum State
        {
            Running,
            Failure,
            Success
        }

        #endregion // Enum

        private void OnStateChanged()
        {
            StateChangedEvent?.Invoke(this, EventArgs.Empty);
        }

        #region Data members

        public State state = State.Running;
        [HideInInspector] public bool isRunning;
        public bool hasRun;
        [HideInInspector] public string guid;
        [HideInInspector] public IBTAgent Agent;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public Blackboard blackboard;

        [Header("Identification")] public string nodeTitle = "";

        [TextArea] public string description = "";

        public event EventHandler StateChangedEvent;

        #endregion // Data members

        #region Public methods

        public Node()
        {
            Init();
        }

        public State Update()
        {
            if (Agent == null)
                return State.Failure;

            hasRun = true;

            if (!isRunning)
            {
                OnStart();
                isRunning = true;
            }

            state = OnUpdate();
            OnStateChanged();

            if (state == State.Success || state == State.Failure)
            {
                OnStop();
                isRunning = false;
            }

            return state;
        }

        #endregion

        #region abstract methods

        protected abstract State OnUpdate();
        protected abstract void OnStart();
        public abstract void OnStop();
        protected abstract void Init();

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        #endregion
    }
}