using UnityEngine;

namespace Moves.Effect
{
    /// <summary> Changes HP according to damage type. </summary>
    [AddComponentMenu("Pokemon Moves/Move Effects/Deal Damage")]
    public class DealDamage : MoveEffect
    {
        [Tooltip("Type of damage to deal.")]
        public DamageType type;

        [Tooltip("Base power of move."), ConditionalHide("type", new int[] { (int)DamageType.Regular })]
        public int power;

        [Tooltip("Amount of HP to remove."), ConditionalHide("type", new int[] { (int)DamageType.Flat })]
        public int flatLoss;

        [Tooltip("Percentage of HP to remove."), ConditionalHide("type", new int[] { (int)DamageType.PercentageOfMax, (int)DamageType.PercentageOfCurrent }, 0, 1)]
        public float percentLoss;

        [Tooltip("Determines if damage is physical or special.")]
        public bool isPhysical;

        protected override void ApplyEffect()
        {
            /* Get User And Target */
            Pokemon user = move.User;
            Pokemon target = move.User.CurrentTarget;

            /* Get Effectivness Mod */
            float effectivnessMod = move.Type.EffectivnessMod(target);

            /* If This Move Is NOT Effective, Print It and Return */
            if (effectivnessMod == 0)
            {
                DialogueManager.instance.StartCoroutine(DialogueManager.Print("No effect." + '\n', false));
                return;
            }

            /* Change The Target's HP */
            target.Hp -= (int)GetDamage(user, target, type);

            /* If This Move Is Super Effective, Print It */
            if (effectivnessMod > 1)
                DialogueManager.instance.StartCoroutine(DialogueManager.Print("Super effective!" + '\n', false));

            /* Else If This Move Is NOT VERY Effective, Print It */
            else if (effectivnessMod < 1)
                DialogueManager.instance.StartCoroutine(DialogueManager.Print("Not very effective." + '\n', false));

            /* If This Move Crited, Print It */
            if (move.Crited)
                DialogueManager.instance.StartCoroutine(DialogueManager.Print("Critcal HIT!" + '\n', false));

        }

        /// <summary> Determines damage of an attack. </summary>
        /// <param name="user"> User of attack. </param>
        /// <param name="target"> Target of attack. </param>
        /// <param name="type"> Type of damage to deal. </param>
        /// <returns> Float representing total damage to give. </returns>
        private float GetDamage(Pokemon user, Pokemon target, DamageType type)
        {
            switch (type)
            {
                case DamageType.Regular:

                    /* Calculate Base Damage Of Move */
                    float damage = (2f * user.Level / 5f * power * (isPhysical ? (float)user.Attack.Value / target.Defense.Value : (float)user.SpecialAttack.Value / target.SpecialDefense.Value)) / 50 + 2;

                    /* Modify Base Damage Of Move */            // ***
                    damage *=                                   // Modifier is:
                        move.Type.EffectivnessMod(target) *     //  The effectivness of this type against the target
                        move.Type.STABMod(user);                //  Bonus if one of the user's type is the same type as this move.

                    /* Return Regular Damage */
                    return damage;

                case DamageType.Flat:
                    return flatLoss;


                case DamageType.PercentageOfCurrent:
                    return target.Hp * percentLoss;

                case DamageType.PercentageOfMax:
                    return target.MaxHp.Value * percentLoss;

                default:
                    Debug.LogError("Invalid damage type: " + type);
                    return 0;

            }
        }
    }

    /// <summary> Damage type of a deal-damage effect. </summary>
    public enum DamageType
    {
        /// <summary> Accounts for type matchups and stats. </summary>
        [Tooltip("Accounts for type matchups and stats.")]
        Regular,

        /// <summary> Flat damage no matter what (except for ineffective matchups). </summary>
        [Tooltip("Flat damage no matter what (except for ineffective matchups).")]
        Flat,

        /// <summary> Percentage damage no matter what (except for ineffective matchups). </summary>
        [Tooltip("Percentage of max HP damage no matter what (except for ineffective matchups).")]
        PercentageOfMax,

        /// <summary> Percentage damage no matter what (except for ineffective matchups). </summary>
        [Tooltip("Percentage of current HP damage no matter what (except for ineffective matchups).")]
        PercentageOfCurrent

    }
}



