using System;
using System.Collections;
using UnityEngine;

namespace Athanor.StateMachine
{
    public interface IStateMachineState
    {
        string name { get; }
        void EnterState();
        void LeaveState();
    }

    public class StateMachine : MonoBehaviour
    {
        protected IStateMachineState currentState = null;
        private bool stateLocked = false;

        protected virtual void EnterCurrentState()
        {
            if (currentState == null)
                Debug.Log("State machine entering null.");
            else
            {
                Debug.Log("State machine entering " + currentState.name + ".");
                currentState.EnterState();
            }
        }

        protected virtual void LeaveCurrentState()
        {
            if (currentState == null)
                Debug.Log("State machine exiting null.");
            else
            {
                Debug.Log("State machine exiting " + currentState.name + ".");
                currentState.LeaveState();
            }
        }

        protected virtual void StateTransition(IStateMachineState newState)
        {
            currentState = newState;
        }

        public void ChangeState(IStateMachineState newState)
        {
            if (stateLocked)
            {
                throw new Exception("Trying to change state while in a state transition!");
            }
            else if (newState == currentState)
            {
                Debug.Log("Neutral state change from/to " + newState.name + ".");
            }
            else
            {
                stateLocked = true;

                LeaveCurrentState();

                StateTransition(newState);

                EnterCurrentState();

                stateLocked = false;
            }
        }
        
        public IEnumerator waitForSteady { get { while (stateLocked) yield return null; } }

        public IEnumerator SteadyChange(IStateMachineState newState)
        {
            yield return waitForSteady;
            ChangeState(newState);
        }
    }
}
