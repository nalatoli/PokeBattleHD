using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class PokeUIState : MonoBehaviour
{
    /* Public Properties */
    public Gradient hpColorPicker;

    /* Private Inspector Properties */
    [SerializeField] private Image portrait_face = null;
    [SerializeField] private new TextMeshProUGUI name = null;
    [SerializeField] private TextMeshProUGUI level = null;
    [SerializeField] private TextMeshProUGUI hpGauge_count = null;
    [SerializeField] private Image hpGauge_primarybar = null;
    [SerializeField] private Image hpGauge_secondarybar = null; 
    [SerializeField] private Image xpGauge_bar = null;
    [SerializeField] private RectTransform statusEffectsContainer = null;
    [SerializeField] private StatusEffectUI statusEffectPrefab = null;

    /* Private Properties */
    private float displayedHP;
    private float displayedXP;
    private List<StatusEffectUI> statusEffects;

    private void Awake()
    {
        /* Initialize New List Of Status Effect Fields */
        statusEffects = new List<StatusEffectUI>();
    }

    /// <summary> Instantly re-writes the pokestate to match all the information about the pokemon. </summary>
    /// <param name="pokemon"> Pokemon to extract the information from. </param>
    public void LoadState(Pokemon pokemon)
    {
        /* Subscribe UI Events To Pokemon */
        pokemon.Events.OnNameChange += SetName;
        pokemon.Events.OnFaceChange += SetFace;
        pokemon.Events.OnLevelChange += SetLevel;
        pokemon.Events.OnHPChange += SetHPGradually;
        pokemon.Events.OnXPChange += SetXPGradually;
        pokemon.Events.OnNameChange += SetStatusEffects;

        /* Set all UI Elements For This Pokemon */
        SetName(pokemon);
        SetFace(pokemon);      
        SetLevel(pokemon);
        SetHP(pokemon);
        SetXP(pokemon);
        SetStatusEffects(pokemon);

    }

    /// <summary> Clear select states of all but one status effect field. </summary>
    /// <param name="effectUI"> Field to enable. </param>
    public void ClearSelectStates(StatusEffectUI effectUI)
    {
        foreach(StatusEffectUI field in statusEffects)
        {
            if (field == effectUI)
                field.SetHover(true);

            else
                field.SetHover(false);
        }          
    }

    /// <summary> Starts hp update routine. </summary>
    /// <param name="pokemon"></param>
    public void SetHPGradually(Pokemon pokemon)
    {
        StartCoroutine(UpdateHP(pokemon));
    }

    /// <summary> Starts xp update routine. </summary>
    /// <param name="pokemon"></param>
    public void SetXPGradually(Pokemon pokemon)
    {
        StartCoroutine(UpdateXP(pokemon));
    }

    /// <summary> Gradually changes the hp count and bar. </summary>
    /// <param name="pokemon"></param>
    public IEnumerator UpdateHP(Pokemon pokemon)
    {
        /* Get Target Ratio */
        float ratio = pokemon.Hp / (float)pokemon.MaxHp.Value;

        /* Scale The Primary Bar and Color The Secondary Bar Depending on Target Ratio */
        hpGauge_primarybar.rectTransform.localScale = new Vector3(ratio, hpGauge_primarybar.rectTransform.localScale.y, 1);
        hpGauge_secondarybar.color = Color.white * 50;

        /* If HP Has Changed */
        while (displayedHP != pokemon.Hp)
        {
            /* Move Display HP Toward to the Reference HP and Update The Count */
            displayedHP = Mathf.MoveTowards(displayedHP, pokemon.Hp, Time.deltaTime * pokemon.MaxHp.Value / 4);
            hpGauge_count.text = (int)displayedHP + "/" + pokemon.MaxHp.Value;

            /* Get The Ratio Between The HP Values */
            ratio = displayedHP / pokemon.MaxHp.Value;

            /* Scale The Secondary Bar and Color The Primary Bar Depending on Current Ratio */
            hpGauge_secondarybar.rectTransform.localScale = new Vector3(ratio, hpGauge_secondarybar.rectTransform.localScale.y, 1);
            hpGauge_primarybar.color = hpColorPicker.Evaluate(ratio);
            yield return null;
        }
    }

    /// <summary> Gradually changes the xp bar. </summary>
    /// <param name="pokemon"> Pokemon to extract information from. </param>
    public IEnumerator UpdateXP(Pokemon pokemon)
    {
        /* If XP Has Changed */
        while (displayedXP != pokemon.Xp)
        {
            /* Move Display XP Toward to the Reference XP and Update The Bar */
            displayedXP = Mathf.MoveTowards(displayedXP, pokemon.Xp, Time.deltaTime);
            xpGauge_bar.rectTransform.localScale = new Vector3(displayedXP, xpGauge_bar.rectTransform.localScale.y, 1);
            yield return null;
        }
    }


    /// <summary> Sets portrait face of pokemon in UI. </summary>
    /// <param name="pokemon"> Pokemon to extract information from. </param>
    private void SetFace(Pokemon pokemon) 
    { 
        portrait_face.sprite = pokemon.Face; 
    }

    /// <summary> Sets name and gender of pokemon in UI. </summary>
    /// <param name="pokemon"> Pokemon to extract information from. </param>
    private void SetName(Pokemon pokemon)
    {
        string genderStr = "";

        if (pokemon.Gender != PokeGender.None)
            genderStr = "  <sprite=" + '"' + pokemon.Gender.ToString() + '"' + " index=0>";

        name.text = pokemon.Name + genderStr;
    }

    /// <summary> Sets level of pokemon in UI.</summary>
    /// <param name="pokemon"> Pokemon to extract information from. </param>
    private void SetLevel(Pokemon pokemon) 
    { 
        level.text = "Lvl " + pokemon.Level; 
    }

    /// <summary> Sets HP count and gauge in UI based on pokemon. </summary>
    /// <param name="pokemon"> Pokemon to extract information from. </param>
    private void SetHP(Pokemon pokemon)
    {
        /* Seperate HP Values By '/' */
        hpGauge_count.text = pokemon.Hp + "/" + pokemon.MaxHp.Value;
        displayedHP = pokemon.Hp;

        /* Get Ratio Of Pokemon's HP */
        float ratio = pokemon.Hp / (float)pokemon.MaxHp.Value;
        
        /* Scale and Color the HP Bars */
        hpGauge_primarybar.rectTransform.localScale = new Vector3(ratio, hpGauge_primarybar.rectTransform.localScale.y, 1);
        hpGauge_secondarybar.rectTransform.localScale = new Vector3(ratio, hpGauge_secondarybar.rectTransform.localScale.y, 1);
        hpGauge_primarybar.color = hpColorPicker.Evaluate(ratio);
        hpGauge_secondarybar.color = hpColorPicker.Evaluate(ratio);

    }
    /// <summary> Sets XP gauge based on pokemon. </summary>
    /// <param name="pokemon"> Pokemon to extract information from. </param>
    private void SetXP(Pokemon pokemon)
    {
        /* Scale and Color the HP Bar */
        xpGauge_bar.rectTransform.localScale = new Vector3(pokemon.Xp, xpGauge_bar.rectTransform.localScale.y, 1);
        displayedXP = pokemon.Xp;
    }

    /// <summary> Re-writes the entire status effect display. </summary>
    /// <param name="pokemon"> Pokemon to extract information from. </param>
    private void SetStatusEffects(Pokemon pokemon)
    {
        /* Remove and Destroy Current Fields */
        for (int i = statusEffects.Count - 1; i >= 0; i--)
        {
            Destroy(statusEffects[i].gameObject);
            statusEffects.RemoveAt(i);
        }

        /* Foreach New Status Effect */
        foreach (StatusEffect effect in pokemon.StatusEffects)
        {
            /* Instantiate A New Field */
            StatusEffectUI field = Instantiate(statusEffectPrefab, statusEffectsContainer);

            /* Fill In The Field */
            field.Load(effect);

            /* Add This Field To List Of Fields */
            statusEffects.Add(field);

        }
    }

}