/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* created for the game project "The Dark Climb" in 2022-2023                */
/* ========================================================================= */

using UnityEngine;

/// <summary>
/// The main difference between an AbstractEnemyAIState and an AbstractState is the addition
/// of a "LogicUpdate()" function that is called less often than the regular "Update()".
/// This function is designed for more computation heavy evaluations, like raycasts.
/// </summary>
public abstract class AbstractEnemyAIState : AbstractState<EnemyAI>
{
    /// <summary>
    /// Use "AI." to access public members of "AbstractEnemyAI" like "lastSeenPlayerPosition",
    /// which is required for stealth-game enemy-behaviour.
    /// </summary>
    protected AbstractEnemyAI AI { get; private set; }

    private float _logicUpdateInterval;
    private float _nextLogicUpdateTimestamp = 0.0f;

    public AbstractEnemyAIState(AbstractEnemyAI enemyStateMachine, float logicUpdateInterval)
    {
        AI = enemyStateMachine;
        _logicUpdateInterval = logicUpdateInterval;
    }

    public override void OnEnter()
    {
        _nextLogicUpdateTimestamp = Time.time + Random.value * _logicUpdateInterval;
        OnStateEnter();
    }

    public override void OnUpdate()
    {
        RegularUpdate();

        if (Time.time > _nextLogicUpdateTimestamp)
        {
            _nextLogicUpdateTimestamp += _logicUpdateInterval;
            LogicUpdate();
        }
    }

    public override void OnExit()
    {
        OnStateExit();
    }

    protected abstract void OnStateEnter();

    protected abstract void OnStateExit();

    protected abstract void RegularUpdate();

    /// <summary>
    /// This function is designed for more computation heavy evaluations, like raycasts,
    /// which should ideally not be meade in the regular "Update()" function every frame.
    /// </summary>
    protected abstract void LogicUpdate();
}