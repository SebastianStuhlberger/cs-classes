/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* created for the game project "The Dark Climb" in 2022-2023                */
/* ========================================================================= */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TraversalMode
{
    Circular,
    BackAndForth
}

/// <summary>
/// Attach this class to a gameObject with 2 or more children.
/// The transforms of those children will then be used to create a traversable path,
/// from which new target locations for AI movement can be read.
/// </summary>
[System.Serializable]
public class MovementPath
{
    [SerializeField] private TraversalMode _traversalMode = TraversalMode.Circular;

    [SerializeField] private Transform _pathParentTransform;

    private Transform[] _availableLocations = null;

#if UNITY_EDITOR
    [SerializeField] private DebugObjectArray _debugAvailableLocations = new();
#endif

    // this index refers to the most recent position that was returned from the _availableLocations array
    private int _lastTargetLocationIndex;

    // this bool tracks the movement direction during the BackAndForth traversal mode
    private bool _currentlyMovingForward = true;

    private void LoadPath()
    {
        List<Transform> childTransforms = new();

        // get the transforms of all immediate children of the _pathParentTransform
        foreach (Transform transform in _pathParentTransform)
        {
            childTransforms.Add(transform);
        }

        _availableLocations = childTransforms.ToArray();

#if UNITY_EDITOR
        _debugAvailableLocations.UpdateArrayPointer(_availableLocations);
#endif

        // validate, if the resulting transforms are enough to create a path
        ValidatePath();
    }

    /// <summary>
    /// Returns the closest available position on the path in world-space.
    /// </summary>
    public Vector3 GetClosestStartingLocation(Vector3 currentSelfPosition)
    {
        if (_availableLocations == null)
        {
            LoadPath();
        }

        if (!ValidatePath())
        {
            return Vector3.zero;
        }

        // initialize with the first stored location
        Vector3 closestLocation = _availableLocations[0].position;
        float closestDistance = Vector3.Distance(currentSelfPosition, closestLocation);

        // check, if any of the remaining locations is closer and update closestLocation accordingly
        for (int locationIndex = 1; locationIndex < _availableLocations.Length; locationIndex++)
        {
            float newDistance = Vector3.Distance(currentSelfPosition, _availableLocations[locationIndex].position);

            if (newDistance < closestDistance)
            {
                closestDistance = newDistance;
                closestLocation = _availableLocations[locationIndex].position;
                _lastTargetLocationIndex = locationIndex;
            }
        }

        return closestLocation;
    }

    public Vector3 GetNextLocation()
    {
        int targetIndex;
        int locationCount = _availableLocations.Length;

        if (_traversalMode == TraversalMode.Circular)
        {
            targetIndex = (_lastTargetLocationIndex + 1) % locationCount;
        }
        else // _traversalMode == TraversalMode.BackAndForth
        {
            if (_currentlyMovingForward)
            {
                targetIndex = (_lastTargetLocationIndex + 1);

                // turn around, if the end of the line was reached
                if (targetIndex >= locationCount)
                {
                    targetIndex = locationCount - 2;
                    _currentlyMovingForward = false;
                }
            }
            else // currentlyMovingBackward
            {
                targetIndex = (_lastTargetLocationIndex - 1);

                // turn around, if the beginning of the line was reached
                if (targetIndex < 0)
                {
                    targetIndex = 1;
                    _currentlyMovingForward = true;
                }
            }
        }

        _lastTargetLocationIndex = targetIndex;

        return _availableLocations[targetIndex].position;
    }

    private bool ValidatePath()
    {
        // check, if there are at least 2 locations available for creating a traversable path
        if (_availableLocations == null || _availableLocations.Length < 2)
        {
            Debug.LogError("MovementPath: No valid path available.");
            return false;
        }

        return true;
    }
}
