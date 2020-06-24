using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moves.Effect
{
    /// <summary> Applies a force to a pokemon's rigidbody. </summary>
    [AddComponentMenu("Pokemon Moves/Move Effects/Change Stat")]
    public class ChangeStat : MoveEffect
    {
        [Tooltip("Stats to change.")]
        public StatType[] stats;

        [Tooltip("Amount of stages to change stat by."), Range(-3, 3)]
        public int stageMod = 1;

        [Tooltip("Chance for stat change to happen."), Range(0,1)]
        public float chance;

        [Tooltip("True for changing stat on self. False for opponent stat change.")]
        public bool onSelf;

        /// <summary> Announcment for this stat change. </summary>
        private string announcment;

        /// <summary> Pokemon to recieve the stat change. </summary>
        private Pokemon recipient;

        /// <summary> Modifier to apply. </summary>
        private StatModifier mod;

        /// <summary> Turn stage modifier into an announcment text. </summary>
        /// <returns> String corrosponding to announcment text. </returns>
        private string GetAnnouncment()
        {
            /* Initilaize Text Recipient */
            string str = recipient.Name + "'s ";

            /* If Only 1 Stat Is Changing, Simply Add The Stat */
            if (stats.Length == 1)
                str += stats[0].ToString();

            /* Else If Only 2 Stats Are Changing, Simply Add The Stats with 'and' Inbetween */
            if (stats.Length == 2) {
                str += stats[0].ToString() + " and ";
                str += stats[1].ToString();
            }
                
            /* Else (More Than 2 Stats) */
            else
            {
                /* Add All Stats but Last Stat */
                for (int i = 0; i < stats.Length - 1; i++)
                    str += stats[i].ToString() + ", ";

                /* Add 'And' To Last Stat */
                str += "and " + stats[stats.Length - 1];
            }


            /* Add Verb Depending On Stat Change Direction */
            if (stageMod > 0)
                str += " rose";
            else
                str += " fell";

            /* Add Adjective Depending On Magnitude Of Stat Change */
            switch(Mathf.Abs(stageMod))
            {
                case 1: return str += "!";
                case 2: return str += " sharply!";
                case 3: return str += " drastrically!";
                default: return str += " stupendously!";
            }
        }

        protected override void ApplyEffect()
        {
            /* Return If Stage Mod Is 0 Or RNG Doesn't Bless */
            if (stageMod == 0 || chance <= Random.value)
                return;
            
            /* Set Modifier */
            mod = PercentModifier.Stage(stageMod);

            /* Set Recipient */
            recipient = onSelf ? move.User : move.User.CurrentTarget;

            /* Generate Announcment Text */
            announcment = GetAnnouncment();

            /* Subscribe Mod Application After This Move Is Finished */
            move.User.Events.AfterMoveUse.Subscribe(ApplyStatChange, EventPriorityLevel.Level2);

        }

        /// <summary> Event after move completion to apply stat change. </summary>
        private IEnumerator ApplyStatChange()
        {
            /* Unsubscribe This Effect So It Doesn't Repeat */
            move.User.Events.AfterMoveUse.Unsubscribe(ApplyStatChange);

            /* Change Stats */
            recipient.AddStatMod(stats, mod);
            
            /* Announce Change */
            yield return BattleManager.Announce(recipient, announcment, true);
            yield return new WaitForSeconds(0.5f);

        }       
    }
}
