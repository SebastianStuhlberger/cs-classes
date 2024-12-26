/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

namespace StateMachine.ExampleClasses;

// create concrete states that belong to a specific subgroup
public class DemoStateB : AbstractState<DemoStateSubgroup>
{
    private const int COUNTER_THRESHOLD = 5;
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
        if (_counter >= COUNTER_THRESHOLD)
        {
            // request changing to another state
            // this will be processed immediately before the next update
            StateMachine.RequestState<DemoStateA>();
        }
    }
}
