using UnityEngine;

/// <summary>
/// Serialized wrapper for spawn settings for a battler.
/// </summary>
[System.Serializable]
public class SpawnSettings
{
    /// <summary> Trainer prefab to use for battler. </summary>
    [Tooltip("Trainer prefab to use for battler.")]
    public Trainer trainer;

    /// <summary> Location in the scene to spawn the trainer. </summary>
    [Tooltip("Location in the scene to spawn the trainer. " +
        "The trainer will be will be positioned and oriented automatically.")]
    public Collider trainerSpawn;

    [Tooltip("Location in the scene to spawn the trainer's pokemon. " +
        "The pokemon will be positioned and oriented automatically.")]
    public Collider pokemonSpawn;

}
