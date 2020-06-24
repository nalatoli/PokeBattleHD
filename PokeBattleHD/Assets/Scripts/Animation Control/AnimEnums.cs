using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary> Animation boolean paramaters. </summary>
public enum AnimBool
{
    /// <summary> Boolean for pokeball to set pokeball open state. </summary>
    IsOpen,

    /// <summary> Boolean for UI to check if mouse is hovering over option. </summary>
    IsHovering,

    /// <summary> Boolean for pokemon to play hurt animation. </summary>
    IsHurt,

    /// <summary> Boolean for trainers returning. </summary>
    Returning,

    /// <summary> Boolean for pokemon that is moving as a result of a hit. </summary>
    IsMoving
}

/// <summary> Animation trigger paramaters. </summary>
public enum AnimTrigger
{
    /// <summary> Trigger for pokemon to cry. </summary>
    Cry,

    /// <summary> Trigger for trainer to throw a ball </summary>
    ThrowBall,

    /// <summary> Trigger for UI to to get mouse click. </summary>
    OnClick,

    /// <summary> Trigger for UI to to exit from selection. </summary>
    Deselect,

    /// <summary> Trigger for pokemon to get up from lying down. </summary>
    GetUp,

    /// <summary> Trigger for characters to do generic actions. </summary>
    DoAction,

    /// <summary> Trigger for pokemon fainting. </summary>
    Faint,

    /// <summary> Trigger for pokemon to perform its eye candy animation. </summary>
    DoEyeCandy,

    /// <summary> Trigger for trainer to perform win animation. </summary>
    Win,

    /// <summary> Trigger for trainer to perform lose animation. </summary>
    Lose,


}

/// <summary> Animation trigger paramaters. </summary>
public enum AnimInt
{
}

/// <summary> Animation float paramaters. </summary>
public enum AnimFloat
{
    /// <summary> Float for getting hurt. X-Axis of force direction. </summary>
    HurtXAxis,

    /// <summary> Float for getting hurt. Y-Axis of force direction. </summary>
    HurtYAxis,

    /// <summary> Float for getting hurt. Z-Axis of force direction. </summary>
    HurtZAxis,
}


