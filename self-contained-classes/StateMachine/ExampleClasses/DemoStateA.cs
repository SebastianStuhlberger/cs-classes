/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

namespace StateMachine.ExampleClasses
{

    // create concrete states that belong to a specific subgroup
    public class DemoStateA : AbstractState<DemoStateSubgroup>
    {
        private const int COUNTER_THRESHOLD = 3;
        private int _counter = 0;

        public override void OnEnter()
        {
            // ensure clean state when entering
            _counter = 0;

            System.Console.WriteLine("EXHIBITING A ========");
        }

        public override void OnExit()
        {
            // cleanup state, if necessary
        }

        public override void OnUpdate()
        {
            System.Console.WriteLine("-------- State A Tick");

            _counter++;
            if (_counter >= COUNTER_THRESHOLD)
            {
                // request for changing a state
                // this will be processed immediately before the next update
                StateMachine.RequestState<DemoStateB>();
            }
        }
    }

}
