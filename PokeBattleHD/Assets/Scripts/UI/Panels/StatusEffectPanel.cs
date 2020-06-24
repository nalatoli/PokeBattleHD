using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectPanel : Panel
{
    /* Public Instance */
    public static StatusEffectPanel instance;

    /* Private Properties */
    public StatusEffect lastStatusEffect;

    protected override void Awake()
    {
        /* Perform Base Awake Function */
        base.Awake();

        /* Initialize Instance (Throw Error If More Than One) */
        if (instance != null)
            Debug.LogWarning("More than one Aliment Data UI in this scene.");
        else
            instance = this;

        /* Initialze Private Properties */
        lastStatusEffect = null;

    }

    /// <summary> Fills move data panel with information about a move. </summary>
    /// <param name="_move"> The move to show information about. </param>
    public void LoadState(StatusEffect _statusEffect)
    {
        /* Return If Aliment Already Loaded */
        if (lastStatusEffect == _statusEffect)
            return;

        /* Record Move So That Loading Doesn't Happen Uneccesarily */
        lastStatusEffect = _statusEffect;

        /* Fill Panel With Status Effect Descriptions */
        FillPanel(_statusEffect.name, _statusEffect.EffectsDescriptions, _statusEffect.Description);

    }

    /// <summary> Returns whether this status effect is loaded into the panel UI. </summary>
    /// <param name="effect"> Effect to check. </param>
    /// <returns> True if this effect is loaded, false if NOT loaded. </returns>
    public bool IsObserving(StatusEffect effect)
    {
        return lastStatusEffect == effect && isObserving;
    }
}
