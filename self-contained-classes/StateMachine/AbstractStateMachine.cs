/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

namespace StateMachine;

/// <summary>
/// This abstract StateMachine is designed for handling systems with multiple states
/// in games by using C# types to refer to said states.
/// </summary>
/// <typeparam name="StateSubGroup">The subgroup class for a StateMachine and its related States.
/// Use these subgroups to group states that belong together.</typeparam>
public abstract class AbstractStateMachine<StateSubGroup> where StateSubGroup : AbstractStateSubgroup
{
    private List<AbstractState<StateSubGroup>> _storedStates = new();

    private AbstractState<StateSubGroup> _currentState = null;
    private AbstractState<StateSubGroup> _requestedState = null;

    private bool IsStateChangeRequested { get => _requestedState != null; }
    private bool StateChangeRequestIsRedundant { get => _requestedState == _currentState; }

    /// <summary>
    /// Check, if the currently active state is of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of state to check the current state against.</typeparam>
    /// <returns>True, if the current state is of the specified type.</returns>
    public bool CurrentStateIs<T>() where T : AbstractState<StateSubGroup>
    {
        ThrowIfUnInitialized();
        return _currentState is T;
    }

    /// <summary>
    /// Check, if the state-machine holds an instance of a specific state type.
    /// </summary>
    /// <typeparam name="T">The type of state to check the held states against.</typeparam>
    /// <returns>True, if the state-machine holds an instance of the specified type.</returns>
    public bool HasStateStored<T>() where T : AbstractState<StateSubGroup>
    {
        return FindStoredState<T>();
    }

    /// <summary>
    /// Call this method to ready the StateMachine for operation and
    /// enter its initial state.
    /// </summary>
    public void Initialize()
    {
        ValidateInitialStoredStates();
        SetInitialState();
    }

    private void ValidateInitialStoredStates()
    {
        if (_storedStates.Count < 1)
        {
            throw new InvalidOperationException("StateMachine initialization failed: ConcreteStateMachine does not contain any states.");
        }
    }

    private void SetInitialState()
    {
        _currentState = _storedStates[0];
        _currentState.OnEnter();
    }

    private void ThrowIfUnInitialized()
    {
        if (_currentState == null)
        {
            throw new InvalidOperationException("StateMachine was not properly initialized. CurrentState is not available.");
        }
    }

    /// <summary>
    /// Call this method once every frame to apply any requested state-switches and
    /// to then trigger the concrete update-behaviour of the current state.
    /// </summary>
    public void Update()
    {
        ThrowIfUnInitialized();

        // switch to another state, if it was requested last frame
        HandleStateRequests();

        // exhibit state-specific update behaviour
        _currentState.OnUpdate();
    }

    private void HandleStateRequests()
    {
        if (IsStateChangeRequested)
        {
            if (StateChangeRequestIsRedundant)
            {
                _requestedState = null;
                return;
            }

            _currentState.OnExit();
            _currentState = _requestedState;
            _requestedState = null;
            _currentState.OnEnter();
        }
    }

    /// <summary>
    /// Use this method to add States to the StateMachine,
    /// if the states can be constructed with an empty constructor via "new()".
    /// </summary>
    /// <typeparam name="T">The concrete Type of the desired State you wish to add.</typeparam>
    protected void AddState<T>() where T : AbstractState<StateSubGroup>, new()
    {
        if (FindStoredState<T>())
        {
            throw new InvalidOperationException($"The State \"{NameOfState<T>()}\" is already stored, duplicate cannot be added.");
        }

        var newState = new T();
        // Inform the State, which StateMachine it belongs to.
        // This step is vital, so include it in all "AddState()" overloads.
        newState.StateMachine = this;

        _storedStates.Add(newState);
    }

    /// <summary>
    /// Use this method to add States to the StateMachine
    /// </summary>
    /// <typeparam name="T">The concrete Type of the State you wish to add.</typeparam>
    /// <param name="state">An Instance of the State you wish to add.</param>
    protected void AddState<T>(T state) where T : AbstractState<StateSubGroup>
    {
        if (FindStoredState<T>(out var storedState))
        {
            throw new InvalidOperationException($"The State \"{NameOfState<T>()}\" is already stored, duplicate cannot be added.");
        }

        // Inform the State, which StateMachine it belongs to.
        // This step is vital, so include it in all "AddState()" overloads.
        state.StateMachine = this;

        _storedStates.Add(state);
    }

    protected void RemoveState<T>() where T : AbstractState<StateSubGroup>
    {
        if (FindStoredState<T>(out var state))
        {
            _storedStates.Remove(state);
        }
        else
        {
            throw new InvalidOperationException($"The State \"{NameOfState<T>()}\" was requested for removal, but is not stored in this StateMachine.");
        }
    }

    /// <summary>
    /// Request a State to change into. Note that only one change of State will occur every Update().
    /// This happens at the start of the Update() and swiches to the last requested State.
    /// </summary>
    /// <typeparam name="T">The Type of State you want to request.</typeparam>
    public void RequestState<T>() where T : AbstractState<StateSubGroup>
    {
        if (FindStoredState<T>(out var state))
        {
            _requestedState = state;
        }
        else
        {
            throw new InvalidOperationException($"A change to State \"{NameOfState<T>()}\" was requested, but the State is not stored in this StateMachine.");
        }
    }

    private bool FindStoredState<T>(out AbstractState<StateSubGroup> state) where T : AbstractState<StateSubGroup>
    {
        state = _storedStates.Find((storedState) => storedState is T);
        return state != null;
    }

    private bool FindStoredState<T>() where T : AbstractState<StateSubGroup>
    {
        return _storedStates.Find((storedState) => storedState is T) != null;
    }

    private string NameOfState<T>() where T : AbstractState<StateSubGroup>
    {
        return typeof(T).Name;
    }
}
