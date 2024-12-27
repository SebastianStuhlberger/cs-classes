/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* created for the game project "The Dark Climb" in 2022-2023                */
/* ========================================================================= */

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This StateMachine is designed to operate based on C# types instead of enums or state-objects.
/// It is also a Monobehaviour, so you can set member fields in the Editor.
/// </summary>
/// <typeparam name="StateSubGroup">The subgroup class for a StateMachine and its related States.
/// Use these subgroups to group states that belong together.</typeparam>
public abstract class AbstractStateMachine<StateSubGroup> : MonoBehaviour where StateSubGroup : AbstractStateSubgroup
{
    private List<AbstractState<StateSubGroup>> _storedStates;

    private AbstractState<StateSubGroup> _currentState;

    // TODO: Align management of requested states with self-contained StateMachine:
    // replace _requestedStateIndex with a nullable _requestedState field

    // note that a value of "-1" means "no new state requested"
    // this could be replaced by a bool, if so desired
    private int _requestedStateIndex = -1;

    public Type CurrentState { get => _currentState.GetType(); private set {; } }

    public virtual void Awake()
    {
        _storedStates = new List<AbstractState<StateSubGroup>>();
        AddDefaultStates();

        if (_storedStates.Count < 1)
        {
            Debug.LogError("ConcreteStateMachine does not add any states", this);
        }
        else
        {
            _currentState = _storedStates[0];
        }
    }

    /// <summary>
    /// Add all desired States for the current StateMachine. <br></br>
    /// Note that the first State, that is added, will also be the initial State for this StateMachine.
    /// </summary>
    protected abstract void AddDefaultStates();

    private void Update()
    {
        _currentState.OnUpdate();
    }

    protected virtual void OnEnable()
    {
        _currentState.OnEnter();
    }

    protected virtual void OnDisable()
    {
        _currentState.OnExit();
    }

    private void LateUpdate()
    {
        if (_requestedStateIndex != -1)
        {
            _currentState.OnExit();
            _currentState = _storedStates[_requestedStateIndex];
            _currentState.OnEnter();

            _requestedStateIndex = -1;
        }
    }

    /// <summary>
    /// Use this method to add States to the StateMachine during "AddDefaultStates()"
    /// </summary>
    /// <typeparam name="T">The concrete Type of the State you wish to add.</typeparam>
    /// <param name="state">An Instance of the State you wish to add.</param>
    protected void AddState<T>(T state) where T : AbstractState<StateSubGroup>
    {
        if (FindStateIndex<T>(out int index))
        {
            // TODO: Add the type of the state causing the warning to the message.
            Debug.LogWarning("State type already stored, cannot add another.", this);
            return;
        }

        // Inform the State, which StateMachine it belongs to.
        // This step is vital, so include it in any other "AddState()" overloads.
        state.stateMachine = this;

        _storedStates.Add(state);
    }

    /// <summary>
    /// Use this method to add States to the StateMachine during "AddDefaultStates()",
    /// if the states can be constructed with an empty constructor via "new()".
    /// </summary>
    /// <typeparam name="T">The concrete Type of the desired State you wish to add.</typeparam>
    protected void AddState<T>() where T : AbstractState<StateSubGroup>, new()
    {
        if (FindStateIndex<T>(out int index))
        {
            // TODO: Add the type of the state causing the warning to the message.
            Debug.LogWarning("State type already stored, cannot add another.", this);
            return;
        }

        var newState = new T();
        newState.stateMachine = this;

        _storedStates.Add(newState);
    }

    protected void RemoveState<T>() where T : AbstractState<StateSubGroup>
    {
        if (FindStateIndex<T>(out int index))
        {
            _storedStates.RemoveAt(index);
        }
        else
        {
            // TODO: Add the type of the state causing the warning to the message.
            Debug.LogWarning("The State that was requested for removal was not found.", this);
            return;
        }
    }

    /// <summary>
    /// Request a State to change into. Note that only one change of State will occur every frame.
    /// This happens at the end of the frame (LateUpdate) and uses the last requested State.
    /// </summary>
    /// <typeparam name="T">The Type of State you want to request.</typeparam>
    public void ChangeState<T>() where T : AbstractState<StateSubGroup>
    {
        if (FindStateIndex<T>(out int index))
        {
            _requestedStateIndex = index;
        }
        else
        {
            // TODO: Add the type of the state causing the warning to the message.
            Debug.LogWarning("The State to which a change was requested was not found.", this);
            return;
        }
    }

    // TODO: Align with self-contained StateMachine: FindStoredState<T>(out state)
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
