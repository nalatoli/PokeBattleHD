using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationCurveExtension
{
    /// <summary> Evaulates curve at normalized time. </summary>
    /// <param name="c"> Curve to evaluate. </param>
    /// <param name="t"> Normalized time [0,1]. </param>
    public static float EvaluateNormalized(this AnimationCurve c, float t)
    {
        /* Warn User If NO Keys Are Present */
        if (c.length < 1)
        {
            Debug.Log("No keys present on " + c.ToString());
            return 0;
        }

        /* Get Duration Of Curve From Last Key Frame */
        float duration = c.keys[c.length - 1].time;

        /* Evaulate At Absolute Time Based Off Normalized Time and Duration Of Curve */
        return c.Evaluate(t * duration);

    }
}
