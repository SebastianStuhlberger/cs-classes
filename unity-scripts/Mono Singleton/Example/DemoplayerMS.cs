/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* created for the game project "The Dark Climb" in 2022-2023                */
/* ========================================================================= */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ############################################################################
// EXAMPLE FILES for MonoSingleton class usage:
//    Demoplayer.cs
//    DemoplayerMS.cs
//    ReferToMonoSingletonFromOtherClass.cs

// INFO:
// Since the MonoSingleton is a wrapper, make sure that it always has acces to the necessary Component,
// by using the [RequireComponent] attribute in front of the derived MonoSingleton class.
// Like this, a MonoSingleton cannot exist on a game-object without the class it is meant to wrap.

[RequireComponent(typeof(Demoplayer))]
public class DemoplayerMS : MonoSingleton<Demoplayer>
{
    // INFO:
    // MonoSingletons should be used as a global wrapper for a designated Component.
    // In the derived MonoSingleton, you can then create static Methods/Properties that use the Instance,
    // as shown with the 3 examples below.

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

    // note that functionalities like this (assuming they are more meaningful)
    // should ideally be moved into the Instance class
    public static void PerformCrazyBackflip(bool backflipGoesWrong)
    {
        Instance.DoBackflip();

        if (backflipGoesWrong)
        {
            Instance.fingerCount -= 2;
            Instance.headTransform = null;
        }
    }

    // INFO:
    // EVERYTHING BELOW THIS POINT IS NOT RECOMMENDED, AND SHOULD ONLY BE USED,
    // IF THERE IS NO ALTERNATIVE WAY TO ACHIEVE A CERTAIN BEHAVIOUR

    protected override void Awake()
    {
        // Insert any custom Awake() behaviour here, if wanted.
        // Make sure to always call base.Awake() beforehand!
        base.Awake();

        // THIS Awake() SHOULD ONLY MODIFY THIS(!) CLASS OBJECT AND NOT THE INSTACE!!

        // THE FOLLOWING LINE OF THIS COMMENT BLOCK IS THEREFORE AN ABSOLUTE NO-GO!!
        // Instance.fingerCount = 10; 
        // Awake() should NOT modifiy the Instance.
        // Move code like that to Instance.Awake()

        // On the other hand, code like this is allowed,
        // as the Instance is unaffected by it.
        if (Camera.main != null)
        {
            hasCamera = true;
        }
    }

    // INFO:
    // MonoSingletons can technically hold their own members, but as established,
    // it is recommended to use MonoSingletons just as a wrapper for .Instance

    public static bool hasCamera = false;
}
