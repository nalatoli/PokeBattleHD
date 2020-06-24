using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAnimationControl : ScriptedAnimationController
{
    /// <summary> Starts open ball animation. </summary>
    public void Open()
    {
        SetAnimBool(AnimBool.IsOpen, true);

    }

    /// <summary> Starts close ball animation. </summary>
    public void Close()
    {
        SetAnimBool(AnimBool.IsOpen, false);

    }
}
