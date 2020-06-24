using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary> Handles events that is specific to a pokemon. </summary>
public class PokemonEventHandler
{
    /// <summary> Pokemon to pass to event listeners. </summary>
    private Pokemon pokemon;

    /// <summary> Create new event handler that is tied to a specified pokemon. </summary>
    /// <param name="pokemon"> Pokemon to tie this event handler to. </param>
    public PokemonEventHandler(Pokemon pokemon)
    {
        this.pokemon = pokemon;
    }

    #region Events from Single Property Changes

    /// <summary> Actions to invoke when the name of a pokemon is changed. </summary>
    public event Action<Pokemon> OnNameChange;
    /// <summary> Invoke actions that listen to a change in name. </summary>
    public void NameChanged() { OnNameChange?.Invoke(pokemon); }

    /// <summary> Actions to invoke when the face of a pokemon is changed. </summary>
    public event Action<Pokemon> OnFaceChange;
    /// <summary> Invoke actions that listen to a change in face. </summary>
    public void FaceChanged() { OnFaceChange?.Invoke(pokemon); }

    /// <summary> Actions to invoke when the battle cry of a pokemon is changed. </summary>
    public event Action<Pokemon> OnBattleCryChange;
    /// <summary> Invoke actions that listen to a change in battle cry. </summary>
    public void BattleCryChanged() { OnBattleCryChange?.Invoke(pokemon); }

    /// <summary> Actions to invoke when a base stat has been changed. </summary>
    public event Action<Pokemon> OnBaseStatChange;
    /// <summary> Invoke actions that listen to a change in base stats. </summary>
    public void BaseStatChanged() { OnBaseStatChange?.Invoke(pokemon); }

    /// <summary> Actions to invoke when a pokemon's gender has been changed. </summary>
    public event Action<Pokemon> OnGenderChange;
    /// <summary> Invoke actions that listen to a change in gender </summary>
    public void GenderChanged() { OnGenderChange?.Invoke(pokemon); }

    /// <summary> Actions to invoke when a pokemon's level has been changed. </summary>
    public event Action<Pokemon> OnLevelChange;
    /// <summary> Invoke actions that listen to a change in level.  </summary>
    public void LevelChanged() { OnLevelChange?.Invoke(pokemon); }

    /// <summary> Actions to invoke when a pokemon's HP has been changed. </summary>
    public event Action<Pokemon> OnHPChange;
    /// <summary> Invoke actions that listen to a change in HP.  </summary>
    public void HPChanged() { OnHPChange?.Invoke(pokemon); }

    /// <summary> Actions to invoke when a pokemon's HP has been changed. </summary>
    public event Action<Pokemon> OnXPChange;
    /// <summary> Invoke actions that listen to a change in HP.  </summary>
    public void XPChanged() { OnXPChange?.Invoke(pokemon); }

    /// <summary> Actions to invoke when a pokemon's ball has been changed. </summary>
    public event Action<Pokemon> OnBallChange;
    /// <summary> Invoke actions that listen to a change in balls.  </summary>
    public void BallChanged() { OnBallChange?.Invoke(pokemon); }

    #endregion

    #region Events From List Changes

    /// <summary> Actions to invoke when the type of a pokemon is changed. </summary>
    public event Action<Pokemon> OnTypeChange;
    /// <summary> Invoke actions that listen to a change in types.  </summary>
    public void TypeChanged() { OnTypeChange?.Invoke(pokemon); }

    /// <summary> Actions to invoke when the status effect of a pokemon is changed. </summary>
    public event Action<Pokemon> OnStatusEffectChange;
    /// <summary> Invoke actions that listen to a change in status effects.  </summary>
    public void StatusEffectChanged() { OnStatusEffectChange?.Invoke(pokemon); }

    #endregion

    #region Sequentially Performed Events

    /// <summary> Event that raises coroutines listeners sequentially before a pokemon uses a move. </summary>
    public RoutinesHandler BeforeMoveUse = new RoutinesHandler();

    /// <summary> Event that raises coroutines listeners sequentially after a pokemon uses a move. </summary>
    public RoutinesHandler AfterMoveUse = new RoutinesHandler();

    #endregion

}


