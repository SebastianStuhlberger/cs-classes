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

public class ReferToMonoSingletonFromOtherClass : MonoBehaviour
{
    // This is an example of how to use a MonoSingleton for globally accessing
    // a wrapped Instance of a certain Component. For this to work, an Instance of 
    // of the wrapped class (Demoplayer) and the wrapping MonoSingleton (DemoplayerMS)
    // need to exist on the same game-object, somewhere in scene of this Component.

    void Start()
    {
        DemoplayerMS.FingerCount = 10;
        DemoplayerMS.PerformCrazyBackflip(true);
    }
}
