using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatModifier
{
    /// <summary> Value used to generate modified stat. </summary>
    public readonly float value;

    public StatModifier(float _value) { value = _value; }

    /// <summary> Apply this modifier to a stat. </summary>
    /// <param name="rawValue"> Value to be modified. </param>
    /// <returns> Modified value. </returns>
    public abstract float Apply(float rawValue);
}

public class LevelModifier : StatModifier
{
    /// <summary> Determines if level modification is altering HP or another stat. </summary>
    private readonly bool isHPMod;

    /// <summary> Level modifier for stat. </summary>
    /// <param name="_value"> Level of pokemon. </param>
    /// <param name="_isHPMod"> Whether to alter the HP stat or another stat. </param>
    public LevelModifier(float _value, bool _isHPMod) 
        : base(_value) 
    {
        isHPMod = _isHPMod;
    }

    public override float Apply(float rawValue)
    {
        if(isHPMod)
            return 10 + value + 2 * rawValue * value / 100;

        return 5 + 2 * rawValue * value / 100;
    }
}

/// <summary> Modifies stats by percentage. </summary>
public class PercentModifier : StatModifier
{
    /// <summary> Percent modifier for stat. </summary>
    /// <param name="_value"> Level of pokemon. </param>
    public PercentModifier(float _value) : base(_value) { }

    public override float Apply(float rawValue) => rawValue * value;

    /// <summary> Stage modifer just like in actual games. </summary>
    /// <param name="stageCount"> Amount of stages.</param>
    /// <returns> Pokemon-like stage modifier. </returns>
    public static PercentModifier Stage(int stageCount) =>
        new PercentModifier(stageCount >= 0 ? 
            ((2 + stageCount) / 2f) : 
            (2f / (2 - stageCount)));

}
