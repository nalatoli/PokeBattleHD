using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaitUntilOrFor : CustomYieldInstruction
{
    Func<bool> predicate;
    float duration;
    float elapsedTime;

    /// <summary> Wait until condition is fufilled or time runs out. </summary>
    /// <param name="_predicate"> Condition to fufill. </param>
    /// <param name="_duration"> Time until time out. </param>
    public WaitUntilOrFor(Func<bool> _predicate, float _duration)
    {
        predicate = _predicate;
        duration = _duration;
        elapsedTime = 0;
    }

    public override bool keepWaiting
    {
        get
        {
            /* Increment Time */
            elapsedTime += Time.deltaTime;
                
            /* If Time Reaches Duration, Resume Coroutine */
            if(elapsedTime > duration)
                return false;

            /* Otherwise, Evalulate Condition */
            return !predicate();
        }
    }
}
