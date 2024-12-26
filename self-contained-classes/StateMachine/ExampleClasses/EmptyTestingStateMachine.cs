/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

namespace StateMachine.ExampleClasses;

// a StateMachine using a given subgroup of states
public class EmptyTestingStateMachine : AbstractStateMachine<DemoStateSubgroup>
{
    public EmptyTestingStateMachine()
    {
        // no calls to AddState or Initialize
    }

    // for testing purposes:
    // a public wrapper for acessing the protected AddState method
    public new void AddState<T>() where T : AbstractState<DemoStateSubgroup>, new()
    {
        base.AddState<T>();
    }
}
