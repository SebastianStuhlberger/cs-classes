/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

namespace StateMachine.ExampleClasses
{

    // a StateMachine using a given subgroup of states
    public class DemoStateMachine : AbstractStateMachine<DemoStateSubgroup>
    {
        public DemoStateMachine()
        {
            // with this order, DemoStateA will be the initial/default state
            AddState<DemoStateA>();
            AddState<DemoStateB>();

            // this state machine initializes on its own, which might not be
            // the desired behaviour for every state machine, but is in this case
            Initialize();
        }
    }

}
