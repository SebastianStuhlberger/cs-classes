/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* created for the game project "The Dark Climb" in 2022-2023                */
/* ========================================================================= */

using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class TaskBasedEnemyAI : AbstractEnemyAI
{
    // keep members public to have easy access to them
    // from within a concrete TaskEnemy_AbstractState

    [Header("Each enemy needs their own set of tasks!")]
    public Transform taskList;

    [Space]
    public DefaultEnemySettings enemySettings;
    public FloatReference logicUpdateInterval;
    public FieldOfViewChecker fieldOfView;
    public NavMeshAgent navMeshAgent;
    public Light eyeLight;

    [Header("Debug, fully optional")]
    [ReadOnly] public string currentTaskDescription;
    [SerializeField] private TextMeshProUGUI _currentTaskDescriptionBox;

    protected override void AddDefaultStates()
    {
        var followTaskState = new TaskEnemy_FollowTaskState(this);
        isStationary = followTaskState.isStationary;

        AddState(followTaskState);
        AddState(new TaskEnemy_ChasePlayerState(this));
        AddState(new TaskEnemy_InvestigateLocation(this));
    }

    public void UpdateTaskDescription(EnemyTask currentTask)
    {
        string description = currentTask.GetDescription();
        currentTaskDescription = description;
        if (_currentTaskDescriptionBox)
        {
            _currentTaskDescriptionBox.text = description;
        }
    }
}

/// <summary>
/// The base state for all states for task-based enemies.
/// Use "TaskAI." to access public members of "TaskBasedEnemyAI" like "enemySettings"
/// </summary>
public abstract class TaskEnemy_AbstractState : AbstractEnemyAIState
{
    protected TaskBasedEnemyAI TaskAI { get; private set; }

    public TaskEnemy_AbstractState(TaskBasedEnemyAI enemyAI)
        : base(enemyAI, enemyAI.logicUpdateInterval.Value)
    {
        TaskAI = enemyAI;
    }

    protected void SetLastSeenPosition()
    {
        bool playerSeen = false;
        Collider[] targets = TaskAI.fieldOfView.targetsInFOV;

        // if any spotted target is the actual player:
        // store their position for further use
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].gameObject.layer == (int)LayerEnum.Player)
            {
                playerSeen = true;
                TaskAI.UpdateLastSeenPlayerPosition(PlayerHandle.position);
                break;
            }
        }

        // if only the player's light(-colliders) were seen:

        if (!playerSeen)
        {
            Vector3 positions = Vector3.zero;
            // calculate average position of all seen playerlightspheres
            foreach (var collider in targets)
            {
                positions += collider.gameObject.transform.position;
            }
            positions /= targets.Length;
            TaskAI.UpdateLastSeenPlayerPosition(positions);
        }
    }
}

public class TaskEnemy_FollowTaskState : TaskEnemy_AbstractState
{
    private EnemyTask[] tasks;
    private int currentTaskIndex;
    private EnemyTask currentTask;

    // true, if the enemy does not have any GoToPostion Tasks
    public bool isStationary { get; private set; }

    public TaskEnemy_FollowTaskState(TaskBasedEnemyAI enemyAI) : base(enemyAI)
    {
        // store the enemy's tasks
        tasks = enemyAI.taskList.GetComponents<EnemyTask>();

        isStationary = true;

        // link all tasks to this enemy's TaskBasedEnemyAI (state machine)
        for (int taskIndex = 0; taskIndex < tasks.Length; taskIndex++)
        {
            tasks[taskIndex].TaskAI = enemyAI;

            //  if the enemy holds any GoToPostion Tasks, it is not stationary
            if (tasks[taskIndex] is EnemyTask_GoToPosition)
            {
                isStationary = false;
            }
        }
    }

    protected override void OnStateEnter()
    {
        currentTaskIndex = 0;

        currentTask = tasks[currentTaskIndex];

#if UNITY_EDITOR
        TaskAI.UpdateTaskDescription(currentTask);
#endif
        currentTask.OnTaskBegin();
    }

    protected override void RegularUpdate()
    {
        // check, if the current task is finished;
        // if so: move on to the next task
        if (currentTask.IsFinished())
        {
            currentTaskIndex++;

            // go back to first task, if all tasks in the list were completed
            if (currentTaskIndex == tasks.Length)
            {
                currentTaskIndex = 0;
            }

            currentTask.OnTaskExit();
            currentTask = tasks[currentTaskIndex];
#if UNITY_EDITOR
            TaskAI.UpdateTaskDescription(currentTask);
#endif
            currentTask.OnTaskBegin();
        }

        currentTask.OnTaskUpdate();
    }

    protected override void OnStateExit()
    {
        currentTask.OnTaskExit();
    }

    protected override void LogicUpdate()
    {
        currentTask.OnTaskLogicUpdate();

        // if awareness hits investigate threshold: switch to investigate player state;
        // this can be triggered entirely through high global awareness.

        bool shouldInvestigatePlayerByAwareness = TaskAI.awareness >= TaskAI.awarenessSettings.investigateAwarenessThreshold;
        if (shouldInvestigatePlayerByAwareness)
        {
            var playerDistance = Vector3.Distance(TaskAI.lastSeenPlayerPosition, TaskAI.transform.position);
            bool playerIsInRange = playerDistance <= TaskAI.awarenessSettings.investigateDistanceThreshold;
            if (playerIsInRange)
            {
                bool playerIsVulnerable = !TaskAI.playerIsSafe;
                if (playerIsVulnerable)
                {
                    AI.ChangeState<TaskEnemy_InvestigateLocation>();
                }
            }
        }

        // if the player or the player's light is seen at any time during task operations, increase the awareness
        if (TaskAI.fieldOfView.targetsInFOV.Length > 0)
        {
            SetLastSeenPosition();

            // increase the awareness quickly, if the actual player is seen
            if (FieldOfViewUtil.IsLayerSeen(LayerEnum.Player, TaskAI.fieldOfView.targetsInFOV))
            {
                TaskAI.ApplySelfAwarenessChange(TaskAI.logicUpdateInterval.Get() * TaskAI.awarenessSettings.selfAwarenessIncreaseSpeedPlayerSeen);
            }
            // increase the awareness slowly, only the player's light is seen
            else
            {
                TaskAI.ApplySelfAwarenessChange(TaskAI.logicUpdateInterval.Get() * TaskAI.awarenessSettings.selfAwarenessIncreaseSpeed);
            }

            // if awareness hits chase threshold switch to chase player state
            if (TaskAI.awareness >= TaskAI.awarenessSettings.chaseAwarenessThreshold)
            {
                if (!TaskAI.playerIsSafe)
                {
                    AI.ChangeState<TaskEnemy_ChasePlayerState>();
                }
            }
        }

        // if the player nor their light is seen during task operation,
        // slowly decrease awareness
        else
        {
            if (TaskAI.selfAwareness > 0)
            {
                TaskAI.ApplySelfAwarenessChange(TaskAI.logicUpdateInterval.Get() * -TaskAI.awarenessSettings.selfAwarenessDepletionSpeed);
            }
        }
    }
}

public class TaskEnemy_ChasePlayerState : TaskEnemy_AbstractState
{
    // code by other team members
    // this section was not programmed by me
}

public class TaskEnemy_InvestigateLocation : TaskEnemy_AbstractState
{
    // code by other team members
    // this section was not programmed by me
}
