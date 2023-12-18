/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* created for the game project "The Dark Climb" in 2022-2023                */
/* ========================================================================= */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteStateTwo : AbstractState<ConcreteStateSubgroup>
{
    private FloatReference _veryImportantValue;

    // NOTE: this custom constructor does not call the base constructor, meaning that the bools
    // "triggerStateEnterOnEnable" and "triggerStateExitOnDisable" will retain their default value "true"
    public ConcreteStateTwo(FloatReference veryImportantValue)
    {
        _veryImportantValue = veryImportantValue;
    }

    public override void OnEnter()
    {
        // Add listeners in OnEnter
        _veryImportantValue.AddListener(OnChanged);
    }

    public override void OnExit()
    {
        // Remove listeners in OnExit
        _veryImportantValue.RemoveListener(OnChanged);
    }

    // INFO:
    // Use "stateMachine." to access public members of the stateMachine, as demonsrated below.
    // Use the below syntax to request a State change at the end of the current frame.
    // As you can see, you only need to specify the Type of the target State.

    private void OnChanged(float value)
    {
        if (value > 5.0f)
        {
            stateMachine.ChangeState<ConcreteStateTwo>();
        }
    }

    public override void OnUpdate()
    {
        // upadte
    }
}
