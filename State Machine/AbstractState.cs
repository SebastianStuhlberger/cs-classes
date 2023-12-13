/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* created for the game project "The Dark Climb" in 2022-2023                */
/* ========================================================================= */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Derive from this class to create a new State to insert into a StateMachine.
/// </summary>
/// <typeparam name="StateSubGroup">The subgroup for the target StateMachine and all its states</typeparam>
public abstract class AbstractState<StateSubGroup> where StateSubGroup : AbstractStateSubgroup
{
    public AbstractStateMachine<StateSubGroup> stateMachine;

    public abstract void OnEnter();

    public abstract void OnExit();

    public abstract void OnUpdate();
}
