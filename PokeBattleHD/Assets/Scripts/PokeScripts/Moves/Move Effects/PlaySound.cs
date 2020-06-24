using UnityEngine;

namespace Moves.Effect
{
    /// <summary> Plays a sound in the soundmanager. </summary>
    [AddComponentMenu("Pokemon Moves/Move Effects/Play Sound")]
    public class PlaySound : MoveEffect
    {
        [Tooltip("Sound to play. This sound is played by the sound manager.")]
        public Sound sound;

        protected override void ApplyEffect()
        {
            /* Play Sound */
            SoundManager.PlaySound(sound);

        }
    }
}
