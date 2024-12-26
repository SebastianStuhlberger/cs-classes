/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* created for the game project "The Dark Climb" in 2022-2023                */
/* ========================================================================= */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this class as a generic wrapper that turns any Component into a Singleton, making it
/// easily accessible within your scene. To do so, derive from this base class and place
/// an Instance of the derived class on the same game-object as the to be wrapped Component.
/// Use [RequireComponent] in deriving classes to guarantee the availability of an Instance.
/// </summary>
/// <typeparam name="T">The Type of the Component that should be wrapped by the deriving class</typeparam>
public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
{
    // the instance only has a dedicated null-check during initialization
    public static T Instance { get; private set; }

    /// <summary>
    /// It is strongly disadvised to override the MonoSingleton.Awake() method in child classes.
    /// Try to instead move Awake() functionality to the Instance.Awake(), if possible.
    /// When overriding, call "base.Awake()" before trying to access the Instance.
    /// </summary>
    protected virtual void Awake()
    {
        // Singleton initialization
        if (Instance == null)
        {
            if (TryGetComponent<T>(out T component))
            {
                Instance = component;
            }
            else
            {
                Debug.LogError("Singleton could not find Component to wrap on this game-object!", this);
            }
        }
        else
        {
            Debug.LogError("Multiple instances of a Singleton detected within this scene. " +
                "This instance will be ignored.", this);
        }
    }
}
