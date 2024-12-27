/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* created for the game project "The Dark Climb" in 2022-2023                */
/* ========================================================================= */

using UnityEngine;

/// <summary>
/// The AbstractStateSubgroup for all EnemyAI related StateMachines and States
/// </summary>
public class EnemyAI : AbstractStateSubgroup { }

/// <summary>
/// This StateMachine makes the most basic information required for our
/// stealth-game-enemies available to any States that are added to it.
/// </summary>
public abstract class AbstractEnemyAI : AbstractStateMachine<EnemyAI>
{
    // -----------------------------------------------------
    // -----------------------------------------------------
    // Fields to set in editor

    [SerializeField] private AwarenessSettings _awarenessSettings;
    [SerializeField] private FloatReference _globalAwareness;
    [SerializeField] private VectorReference _lastSeenPlayerPosition;
    [SerializeField] private AnimationCurve _distanceToPlayerCurve;
    [SerializeField] private BoolReference _isInActiveSafeHavenZone;

    // -----------------------------------------------------
    // -----------------------------------------------------
    // Externally available player-data

    // TODO: use PascalCase for props
    public Vector3 lastSeenPlayerPosition => _lastSeenPlayerPosition.Value;
    public bool playerIsSafe => _isInActiveSafeHavenZone.Value;

    // -----------------------------------------------------
    // -----------------------------------------------------
    // Enemy positioning data

    // TODO: use PascalCase for props
    public bool isStationary { get; protected set; } = false;
    public Vector3 initialPosition { get; private set; }
    public Quaternion initialRotation { get; private set; }

    // -----------------------------------------------------
    // -----------------------------------------------------
    // Awareness management

    // TODO: use PascalCase for props
    public AwarenessSettings awarenessSettings => _awarenessSettings;
    public float globalAwareness => _globalAwareness.Value;
    public float selfAwareness { get; protected set; } = 0;
    public float awareness { get { return selfAwareness + _globalAwareness.Value; } }

    // -----------------------------------------------------
    // -----------------------------------------------------
    // Player distance tracking

    // TODO: use PascalCase for props
    private float _distanceToPlayer
    {
        get => Vector3.Distance(gameObject.transform.position, PlayerHandle.position);
    }

    // This factor can be used to access a value in range (0.0, 1.0) that expresses if the player
    // is nearby or far away. A high value means the player is close.
    private float _distanceToPlayerMultiplicator
    {
        get
        {
            var playerDistanceFactor = _distanceToPlayer / _awarenessSettings.maximumTrackDistance;
            return _distanceToPlayerCurve.Evaluate(
                Mathf.Max(0.0f, 1.0f - playerDistanceFactor)
            );
        }
    }

    // -----------------------------------------------------
    // -----------------------------------------------------
    // Member functions

    public override void Awake()
    {
        base.Awake();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        selfAwareness = 0;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        selfAwareness = 0;
    }

    /// <summary>
    /// Resets the enemy to its original position.
    /// </summary>
    /// <param name="resetStationaryPositions">By default, enemies that are stationary do not have their positions reset.</param>
    public void ResetTransform(bool resetStationaryPositions = false)
    {
        if (!isStationary || resetStationaryPositions)
        {
            transform.position = initialPosition;
        }
        transform.rotation = initialRotation;
    }

    public void ApplySelfAwarenessChange(float value)
    {
        if (value > 0)
        {
            value *= _distanceToPlayerMultiplicator;

            // make sure that self awareness can only be filled up to max value
            float maximumApplicableAwareness = _awarenessSettings.selfAwarenessMaximum - selfAwareness;
            float clampedSelfAwarenessToBeApplied = Mathf.Min(
                value,
                maximumApplicableAwareness
            );
            selfAwareness += clampedSelfAwarenessToBeApplied;

            // make sure that global awareness can only be filled up to max value
            // only transfer a portion of the selfAwareness to the globalAwareness
            float maximumApplicableGlobalAwareness = _awarenessSettings.globalAwarenessMaximum - globalAwareness;
            float scaledGlobalAwareness = value * _awarenessSettings.globalAwarenessTransferPercentage;
            float clampedGlobalAwarenessToBeApplied = Mathf.Min(
                scaledGlobalAwareness,
                maximumApplicableGlobalAwareness
            );
            _globalAwareness.ChangeBy(clampedGlobalAwarenessToBeApplied);
        }

        else if (value < 0)
        {
            // make sure that selfAwareness can never be lower than 0
            selfAwareness = Mathf.Max(selfAwareness + value, 0);
        }

    }

    public void UpdateLastSeenPlayerPosition(Vector3 newLastSeenPlayerPosition)
    {
        _lastSeenPlayerPosition.Value = newLastSeenPlayerPosition;
    }
}
