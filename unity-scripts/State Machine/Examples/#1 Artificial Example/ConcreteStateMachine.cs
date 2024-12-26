/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* created for the game project "The Dark Climb" in 2022-2023                */
/* ========================================================================= */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// create a subgroup for the current state machine and its states, in order to group them
public class ConcreteStateSubgroup : AbstractStateSubgroup { }

public class ConcreteStateMachine : AbstractStateMachine<ConcreteStateSubgroup>
{
    [SerializeField] private FloatReference fillThisFieldInEditor;

    // Override "AddDefaultStates()" like below and add your concrete states.
    // Ideally, all your states are independant

    protected override void AddDefaultStates()
    {
        AddState<ConcreteStateOne>();
        AddState<ConcreteStateTwo>(new(fillThisFieldInEditor));
    }
}
