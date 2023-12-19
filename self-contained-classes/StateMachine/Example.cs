using System;

namespace StateMachine
{
    class Example
    {
        private const int MAIN_LOOP_TICKS = 32;
        private const int SIDE_LOOP_TICKS = 5;

        static void Main()
        {
            // constructors can definitely get more complex than this
            DemoStateMachine machine = new();
            machine.Initialize();

            // example update operation
            for (int i = 0; i < MAIN_LOOP_TICKS; i++)
            {
                machine.Update();
            }

            // requesting state switches from outside the StateMachine
            // after the above iterations, the machine would go on with state A,
            // so let's change it to B instead
            machine.RequestState<DemoStateB>();

            // example update operation
            for (int i = 0; i < SIDE_LOOP_TICKS; i++)
            {
                machine.Update();
            }

            // other methods:
            Console.WriteLine("CurrentState     = " + machine.CurrentState);
            Console.WriteLine("StoredStateCount = " + machine.StoredStates.Count);
        }
    }
}
