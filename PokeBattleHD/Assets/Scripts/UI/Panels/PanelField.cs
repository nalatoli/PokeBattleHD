using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PanelField : MonoBehaviour
{
    [SerializeField] private Image identifier = null;
    [SerializeField] private TextMeshProUGUI identifier_text = null;
    [SerializeField] private Image description = null;
    [SerializeField] private TextMeshProUGUI description_text = null;
    [SerializeField] private TextMeshProUGUI subtext = null;
    [SerializeField] private Image tooltip = null;
    [SerializeField] private TextMeshProUGUI tooltip_text = null;

    private IEnumerator routineInstance;
    private const float refAlpha = 0.7f;
    private bool tooltipState;
    private bool tooltipActivatable;

    private void Awake()
    {
        tooltipState = false;
        tooltip.gameObject.SetActive(false);
    }


    /// <summary> Fills this field with information about a move effect. </summary>
    /// <param name="effect"> Effect's description to place onto field. </param>
    public void FillField(EffectDescription effect)
    {
        /* Update Text */
        identifier_text.text = effect.identifier;
        description_text.text = effect.description;
        tooltip_text.text = effect.toolTipDescription;

        /* Update Description Color */
        if (effect.useCustomColor)
            description.color = new Color(
                effect.descriptionColor.r, 
                effect.descriptionColor.g, 
                effect.descriptionColor.b, 
                description.color.a);

        /* Update Subtext */
        if (effect.useSubtext)
        {
            subtext.text = effect.subtext;
            subtext.color = new Color(
                effect.subtextColor.r,
                effect.subtextColor.g,
                effect.subtextColor.b,
                subtext.color.a);
        }

        /* Else Clear Subtext */
        else
            subtext.text = "";

        /* Record Whether The Tooltip Is Activatable */
        tooltipActivatable = effect.useToolTip;

    }

    public void Click()
    {
        /* Return If Tooltip Is Disabled */
        if (!tooltipActivatable)
            return;

        /* Set Tooltip */
        SoundManager.PlaySound("Blip");
        SetTooltip(!tooltipState);    
    }

    /// <summary> Sets state of tool tip. </summary>
    /// <param name="state"> True for active and false for inactive. </param>
    public void SetTooltip(bool state)
    {
        /* Return If Tooltip Is Disabled */
        if (!tooltipActivatable)
            return;

        /* If Setting Is True, Hide All Other Tool Tips */
        if (state == true) {
            MovePanel.instance.HideAllFieldTooltips();
            StatusEffectPanel.instance.HideAllFieldTooltips();
        }

        /* Set State Of Tooltip and Disable Hover Mode */
        tooltip.gameObject.SetActive(state);
        tooltipState = state;
        SetHover(false);

    }

    public void SetHover(bool state)
    {
        /* Return If Tooltip Is Disabled */
        if (!tooltipActivatable)
            return;

        /* Start Alpha Change If State Is True */
        if (state == true)
            this.StartInterruptableCoroutine(identifier.FluxAlpha(1, 0.2f), ref routineInstance);
       

        /* Else Stop Alpha Change */
        else 
            this.StartInterruptableCoroutine(identifier.ChangeAlphaOvertime(1, 1f), ref routineInstance);
        
    }

}

[System.Serializable]
/// <summary> Effect description for UI fields. </summary>
public class EffectDescription
{
    [Tooltip("Text used to indicate what kind of effect this is. (i.e. 'Power' for damage dealing moves")]
    /// <summary> Text used to indicate what kind of effect this is. </summary>
    public string identifier = "Power";

    [Tooltip("Text used to describe the field identifier. (i.e. '40' for damage dealing moves")]
    /// <summary> Text used to describe the field identifier. </summary>
    public string description = "40";

    [Tooltip("Determines if custom color is used for description background.")]
    /// <summary> Determines if subtext is visible. </summary>
    public bool useCustomColor = false;

    [Tooltip("Color used as background for the field description. (NOTE: Text color is WHITE)"),
         ConditionalHide("useCustomColor", false)]
    /// <summary> Color used as background for the field description. </summary>
    public Color descriptionColor = Color.white;

    [Tooltip("Determines if subtext is visible. Subtext is additional info that appears on the " +
        "bottom right of the field.")]
    /// <summary> Determines if subtext is visible. </summary>
    public bool useSubtext = false;

    [Tooltip("Text displayed on bottom right of field. (i.e. x2 for moves that do something twice."),
        ConditionalHide("useSubtext", false)]
    /// <summary> Text displayed on bottom right of field. </summary>
    public string subtext = "x2";

    [Tooltip("Vertex color for subtext. This is the color of the text."), ConditionalHide("useSubtext", false)]
    /// <summary> Vertex color for subtext. This is the color of the text." </summary>
    public Color subtextColor = Color.white;

    [Tooltip("Determines if tooltip is activatable. Tooltips are extra info that appears if the player " +
    "clicks on the field.")]
    public bool useToolTip = false;

    [Tooltip("Text that will be displayed when an extended desription is requested " +
        "(i.e. 'Deals damage to the opponent' for moves that deal damage." +
        "Leaving this blank will make no tooltip appear for this field."), ConditionalHide("useToolTip", false)]
    /// <summary> Description of move effect for UI. </summary>
    public string toolTipDescription = "Deals damage to opponent";
}
