/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* created for the game project "The Dark Climb" in 2022-2023                */
/* ========================================================================= */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ============================================================================
// EXAMPLE FILES for MonoSingleton class usage:
//    Demoplayer.cs
//    DemoplayerMS.cs
//    ReferToMonoSingletonFromOtherClass.cs

// Since the MonoSingleton is a wrapper, make sure that it always has acces to the necessary Component
// by using the [RequireComponent] attribute for the derived MonoSingleton class.

[RequireComponent(typeof(Demoplayer))]
public class DemoplayerMS : MonoSingleton<Demoplayer>
{
    // MonoSingletons should be used as a global wrapper for a designated Component.
    // In the derived MonoSingleton you can then create static Methods/Properties
    // that use the Instance, as shown below.

    public static Transform Head
    {
        get { return Instance.headTransform; }
        private set { Instance.headTransform = value; }
    }

    public static int FingerCount
    {
        get { return Instance.fingerCount; }
        set { Instance.fingerCount = value; }
    }

    // Functionalities like this (assuming they are more meaningful)
    // should typically be moved into the Instance class.
    // Keep separation of concerns in mind, when adding ANY funcitonality here.
    // MonoSingletons should be purely focused on being a component-wrapper.
    public static void PerformCrazyBackflip(bool backflipGoesWrong)
    {
        Instance.DoBackflip();

        if (backflipGoesWrong)
        {
            Instance.fingerCount -= 2;
            Instance.headTransform = null;
        }
    }

    // EVERYTHING BELOW THIS POINT IS STRONGLY DISADVISED, AND SHOULD ONLY BE USED,
    // IF THERE IS NO ALTERNATIVE WAY TO ACHIEVE A CERTAIN BEHAVIOUR

    protected override void Awake()
    {
        // Make sure to always call base.Awake() beforehand!
        base.Awake();

        // Insert any custom Awake() behaviour here, if wanted.
        // Separation of concerns is the definitive guiding principle here.
        // THIS Awake() should only modify THIS(!) class object and NOT the Instance!

        // THE FOLLOWING LINE OF THIS COMMENT BLOCK IS THEREFORE AN ABSOLUTE NO-GO!
        // Instance.fingerCount = 10; 
        // Awake() should NOT modifiy the Instance.
        // Move code like this to Instance.Awake()

        // On the other hand, code like this is acceptable, if absolutely necessary,
        // as the Instance stays unaffected by it.
        if (Camera.main != null)
        {
            hasCamera = true;
        }
    }

    // MonoSingletons can technically hold their own members, but as established, it is
    // strongly recommended to just use MonoSingletons just as a wrapper for .Instance

    public static bool hasCamera = false;
}
