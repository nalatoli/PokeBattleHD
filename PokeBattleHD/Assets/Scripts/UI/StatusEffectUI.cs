using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectUI : MonoBehaviour
{
    [Tooltip("Reference to status effect icon."), SerializeField]
    private Image icon = null;

    /// <summary> Reference to border image. </summary>
    private Image border;

    /* Private Properties */
    private IEnumerator r1, r2;
    private StatusEffect statusEffect;

    public void Load(StatusEffect effect)
    {
        /* If Effect Already Loaded, Return */
        if (effect == statusEffect)
            return;

        /* Get Component, Initialize Observation State, And Record Effect */
        border = GetComponent<Image>();
        statusEffect = effect;

        /* Load Aestetics */
        icon.sprite = effect.Sprites[0];
        border.color = effect.BorderColor;
        border.SetAlpha(0.7f);
    }

    /// <summary> Sets select state of circle. </summary>
    public void SelectCircle()
    {
        /* Make Blip Sound */
        SoundManager.PlaySound("Blip");

        /* If This Move Is NOT Being Observed */
        if (!StatusEffectPanel.instance.IsObserving(statusEffect))
        {
            /* Disable Hovering, Reanimate The Effect, Enable The Panel, and Load This Status Effect */
            SetHover(false);
            this.StartInterruptableCoroutine(AnimateUntilDeselect(), ref r1);      
            StatusEffectPanel.instance.SetVisibility(true);
            StatusEffectPanel.instance.LoadState(statusEffect);
            
        }

        /* Else (This Move Is Already Being Observed) */
        else
        {
            /* Enable Hover and Disable The Panel */
            StatusEffectPanel.instance.SetVisibility(false);
            SetHover(true);
        }
    }

    /// <summary> Animates icon until this effect is not being observed anymore. </summary>
    private IEnumerator AnimateUntilDeselect()
    {
        this.StartInterruptableCoroutine(icon.AnimateSpritesheet(statusEffect.Sprites, 0.5f), ref r2);
        yield return new WaitUntil(() => StatusEffectPanel.instance.IsObserving(statusEffect));
        yield return new WaitUntil(() => !StatusEffectPanel.instance.IsObserving(statusEffect));
        this.StartInterruptableCoroutine(icon.AnimateSpritesheetToStop(statusEffect.Sprites), ref r2);
    }

    /// <summary> Sets hover state of circle. Will not do anything if this effect is being observed. </summary>
    /// <param name="state"> True for hover and false for still. </param>
    public void SetHover(bool state) 
    {
        /* Don't Do Anything If This Status Effect Is Being Observed */
        if (StatusEffectPanel.instance.IsObserving(statusEffect))
            return;

        /* Start Alpha Change If State Is True */
        if (state == true) {
            this.StartInterruptableCoroutine(border.FluxAlpha(0.7f, 0.3f), ref r1);
            this.StartInterruptableCoroutine(icon.AnimateSpritesheet(statusEffect.Sprites, 0.5f), ref r2);
        }

        /* Else Stop Alpha Change  */
        else {
            this.StartInterruptableCoroutine(border.ChangeAlphaOvertime(0.7f, 1f), ref r1);
            this.StartInterruptableCoroutine(icon.AnimateSpritesheetToStop(statusEffect.Sprites), ref r2);
        }
    }

}
