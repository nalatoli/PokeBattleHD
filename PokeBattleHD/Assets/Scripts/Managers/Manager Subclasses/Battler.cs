using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// Representation of a battler's properties and abilities.
/// Interfaces with other managers. 
/// </summary>
public class Battler
{
    /// <summary> Trainer gameobject to reference. </summary>       
    public Trainer trainer;

    /// <summary> Defines location where new pokemon are spawned in. </summary> 
    public Collider pokeSpawner;

    /// <summary> Reference to trainer's current pokemon. </summary> 
    public Pokemon pokemon;

    /// <summary> 
    /// Generate a battler from user-defined spawn settings.
    /// A trainer will be instantiated from the prefab defined in the settings. </summary>
    /// <param name="settings"> User-defined spawn settings to use. </param>
    public Battler(SpawnSettings settings)
    {
        trainer = Object.Instantiate(settings.trainer, settings.trainerSpawn.transform);
        trainer.name = settings.trainer.name;
        pokeSpawner = settings.pokemonSpawn;
        pokemon = trainer.CurrentPokemon;
    }

}
