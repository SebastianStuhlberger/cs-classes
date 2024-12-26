/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

namespace StateMachine
{

    // the StateMachine for the given subgroup
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
