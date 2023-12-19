/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMachine
{

    /// <summary>
    /// This abstract StateMachine is designed for handling systems with multiple states in games.
    /// It operates based on C# types instead of enums or state-objects.
    /// </summary>
    /// <typeparam name="StateSubGroup">The subgroup class for a StateMachine and its related States.
    /// Use these subgroups to group states that belong together.</typeparam>
    abstract class AbstractStateMachine<StateSubGroup> where StateSubGroup : AbstractStateSubgroup
    {
        // all states that are currently held by the StateMachine
        private List<AbstractState<StateSubGroup>> _storedStates;

        private AbstractState<StateSubGroup> _currentState;

        // note that a value of "-1" means "no new state requested"
        // this behaviour could be replaced by a bool, if so desired
        private int _requestedStateIndex = -1;

        /// <summary>
        /// Returns a list of all held states.
        /// Don't overuse this to keep List creations low.
        /// </summary>
        public List<Type> StoredStates { get => _storedStates.Select(obj => obj.GetType()).ToList(); private set {; } }

        public Type CurrentState { get => _currentState.GetType(); private set {; } }

        protected AbstractStateMachine()
        {
            _storedStates = new List<AbstractState<StateSubGroup>>();
        }

        /// <summary>
        /// Call this method once every frame to trigger the concrete update-behaviour of the current state,
        /// and to to apply any requested state-switches after said update.
        /// </summary>
        public void Update()
        {
            // switch to another state, if it was requested last frame
            HandleStateRequests();
            
            // exhibit state-specific update behaviour
            _currentState.OnUpdate();
        }

        /// <summary>
        /// Call this method to ready the StateMachine for operation and
        /// enter its initial state.
        /// </summary>
        public void Initialize()
        {
            // validate state setup
            if (_storedStates.Count < 1)
            {
                throw new Exception("ConcreteStateMachine does not add any states");
            }
            else
            {
                _currentState = _storedStates[0];
                _currentState.OnEnter();
            }
        }

        private void HandleStateRequests()
        {
            // only switch, if there was actually a state requested
            if (_requestedStateIndex != -1)
            {
                _currentState.OnExit();
                _currentState = _storedStates[_requestedStateIndex];
                _currentState.OnEnter();

                _requestedStateIndex = -1;
            }
        }

        /// <summary>
        /// Use this method to add States to the StateMachine,
        /// if the states can be constructed with an empty constructor via "new()".
        /// </summary>
        /// <typeparam name="T">The concrete Type of the desired State you wish to add.</typeparam>
        protected void AddState<T>() where T : AbstractState<StateSubGroup>, new()
        {
            if (FindStateIndex<T>(out int index))
            {
                throw new Exception("State type already stored, cannot add another.");
            }

            var newState = new T();
            newState.stateMachine = this;

            _storedStates.Add(newState);
        }

        /// <summary>
        /// Use this method to add States to the StateMachine
        /// </summary>
        /// <typeparam name="T">The concrete Type of the State you wish to add.</typeparam>
        /// <param name="state">An Instance of the State you wish to add.</param>
        protected void AddState<T>(T state) where T : AbstractState<StateSubGroup>
        {
            if (FindStateIndex<T>(out int index))
            {
                throw new Exception("State type already stored, cannot add another.");
            }

            // Inform the State, which StateMachine it belongs to.
            // This step is vital, so include it in any other "AddState()" overloads.
            state.stateMachine = this;

            _storedStates.Add(state);
        }

        protected void RemoveState<T>() where T : AbstractState<StateSubGroup>
        {
            if (FindStateIndex<T>(out int index))
            {
                _storedStates.RemoveAt(index);
            }
            else
            {
                throw new Exception("The State that was requested for removal was not found.");
            }
        }

        /// <summary>
        /// Request a State to change into. Note that only one change of State will occur every Update().
        /// This happens at the start of the Update() and swiches to the last requested State.
        /// </summary>
        /// <typeparam name="T">The Type of State you want to request.</typeparam>
        public void RequestState<T>() where T : AbstractState<StateSubGroup>
        {
            if (FindStateIndex<T>(out int index))
            {
                _requestedStateIndex = index;
            }
            else
            {
                throw new Exception("The State to which a change was requested was not found.");
            }
        }

        private bool FindStateIndex<T>(out int indexOfState) where T : AbstractState<StateSubGroup>
        {
            // check, if state type is stored
            for (int i = 0; i < _storedStates.Count; i++)
            {
                if (_storedStates[i].GetType() == typeof(T))
                {
                    indexOfState = i;
                    return true;
                }
            }

            indexOfState = -1;
            return false;
        }
    }

}
