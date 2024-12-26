/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

namespace StateMachine
{

    // declare a subgroup for states and their StateMachine
    class DemoStateMachine : AbstractStateMachine<DemoStateSubgroup>
    {
        public DemoStateMachine()
        {
            // with this order, DemoStateA will be the initial/default state
            AddState<DemoStateA>();
            AddState<DemoStateB>();
        }
    }

}
