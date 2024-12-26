/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

using StateMachine;
using StateMachine.ExampleClasses;

namespace Tests.StateMachine.TestingClasses;

/// <summary>
/// An <see cref="AbstractStateMachine{DemoStateSubgroup}"/> implementation without 
/// predefined states that provides access to some protected methods for testing purposes.
/// </summary>
internal sealed class EmptyTestingStateMachine : AbstractStateMachine<DemoStateSubgroup>
{
    public new void AddState<T>() where T : AbstractState<DemoStateSubgroup>, new()
    {
        base.AddState<T>();
    }

    public new void AddState<T>(T state) where T : AbstractState<DemoStateSubgroup>
    {
        base.AddState<T>(state);
    }

    public new void RemoveState<T>() where T : AbstractState<DemoStateSubgroup>
    {
        base.RemoveState<T>();
    }
}
