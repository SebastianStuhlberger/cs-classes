/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

using StateMachine.ExampleClasses;

namespace StateMachine;

class Example
{
    private const int MAIN_LOOP_TICKS = 17;
    private const int SIDE_LOOP_TICKS = 5;

    static void Main()
    {
        try
        {
            // constructors can definitely get more complex than this
            DemoStateMachine machine = new();

            // example update operation
            for (int i = 0; i < MAIN_LOOP_TICKS; i++)
            {
                machine.Update();
            }

            // requesting state switches from outside the StateMachine;
            // after the above iterations, the machine would go on with state A,
            // so let's change it to B instead
            machine.RequestState<DemoStateB>();
            Console.WriteLine("REQUESTED: B ========");

            // example update operation
            for (int i = 0; i < SIDE_LOOP_TICKS; i++)
            {
                machine.Update();
            }

            // example calls of other methods
            Console.WriteLine("==================================");
            Console.WriteLine("CurrentStateIs<DemoStateA> = " + machine.CurrentStateIs<DemoStateA>());
            Console.WriteLine("HasStateStored<DemoStateB> = " + machine.HasStateStored<DemoStateB>());
        }
        catch (Exception exception)
        {
            Console.WriteLine($"EXCEPTION OCCURED: {exception.Message}");
            Console.WriteLine($"STACK TRACE: {exception.StackTrace}");
        }
    }
}
