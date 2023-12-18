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
// This class is just an example for a Component.
// It is wrapped by a MonoSingleton in DemoPlayerMS.cs

public class Demoplayer : MonoBehaviour
{
    public Transform headTransform;

    public int fingerCount;

    public void DoBackflip()
    {
        // code
    }
}
