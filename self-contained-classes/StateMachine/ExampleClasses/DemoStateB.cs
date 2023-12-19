/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

namespace StateMachine {

    // create concrete states that belong to a specific subgroup
    class DemoStateB : AbstractState<DemoStateSubgroup>
    {
        private const int SWITCH_COUNTER = 5;
        private int _counter = 0;

        public override void OnEnter()
        {
            // ensure clean state when entering
            _counter = 0;

            System.Console.WriteLine("EXHIBITING B ========");
        }

        public override void OnExit()
        {
            // cleanup state, if necessary
        }

        public override void OnUpdate()
        {
            System.Console.WriteLine("-------- State B Tick");
            
            _counter++;
            if (_counter >= SWITCH_COUNTER)
            {
                // request for changing a state
                // this will be processed immediately before the next update
                stateMachine.RequestState<DemoStateA>();
            }
        }
    }

}
