using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Runtime stat of pokemon. 
/// Value is used in various calculations including move priority,
/// damage taken, damage dealt, etc.
/// </summary>
public class Stat
{
    /// <summary> Base value of stat. </summary>
    public int BaseValue { get; private set; }

    /// <summary> Current value of stat. </summary>
    public int Value { get; private set; }

    /// <summary> Color of particles upon stat change. </summary>
    public Color Color { get; private set; }

    /// <summary> Generates poke stat that influences various battle mechanics. </summary>
    /// <param name="baseValue"> The species-dependent base value of this stat. </param>
    /// <param name="level"> THe instance-dependent level of the pokemon with this stat. </param>
    /// <param name="isHPStat"> Determines if this stat is an HP stat (calcualted differently). </param>
    /// <param name="color"> Color of particles on stat change. </param>
    public Stat(int baseValue, int level, bool isHPStat, Color color)
    {
        BaseValue = baseValue;
        Value = (int)new LevelModifier(level, isHPStat).Apply(baseValue);
        Color = color;
    }

    /// <summary> Adds a stat modifier to this stat and plays particle effects over time. </summary>
    /// <param name="mod"> The modifier to apply to this stat. </param>
    public void AddModifier(StatModifier mod)
    {
        /* Get New Value */
        Value = (int)mod.Apply(Value);
    }

}
