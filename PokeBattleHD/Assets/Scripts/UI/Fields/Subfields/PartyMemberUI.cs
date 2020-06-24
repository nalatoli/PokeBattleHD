using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyMemberUI : OptionFieldChild
{
    /* Public Properties */
    public Gradient hpColorPicker;

    /* References */
    [SerializeField] private GameObject info_Container = null;
    [SerializeField] private Image portrait_Container = null;
    [SerializeField] private Image portrait_face = null;
    [SerializeField] private new TextMeshProUGUI name = null;
    [SerializeField] private TextMeshProUGUI level = null;
    [SerializeField] private Image hpGauge_bar = null;
    [SerializeField] private Image xpGauge_bar = null;
    [SerializeField] private RectTransform statusEffectsContainer = null;
    [SerializeField] private StatusEffectUI statusEffectPrefab = null;
    [SerializeField] private Image chooseButton = null;

    /// <summary> Status this UI is referencing. </summary>
    public PokemonInstanceStatus Status { get; private set; }

    /// <summary> List Of Status Effect UI Elemnts Currently Instanced </summary>
    private List<StatusEffectUI> statusEffects;
    private IEnumerator scaleRoutine, portraitRoutine, faceRoutine, chooseRoutine;
    private bool isLoaded;

    Color c1 = new Color(0.7f, 0.7f, 0.7f, 1);
    Color c2 = new Color(0.4f, 0.4f, 0.4f, 1);

    protected override void Awake()
    {
        /* Perform Base Awake Functionality */
        base.Awake();

        /* Intialize List Of Status Effects */
        if (statusEffects == null)
            statusEffects = new List<StatusEffectUI>();
        isLoaded = false;
    }

    protected override void OnEnable()
    {
        /* Perform Base On Enable Functionality */
        base.OnEnable();

        /* If Info Is NOT Active */
        if(!info_Container.activeInHierarchy)
        {
            /* Set Default Portrait Size and Start Changing Its Color */
            portrait_Container.transform.localScale = Vector3.one;
            this.StartInterruptableCoroutine(
                portrait_Container.FluxColor(c1, c2, 1), ref portraitRoutine);

            /* Start Changing Face's Color */
            this.StartInterruptableCoroutine(
                portrait_face.FluxColor(c1, c2, 1), ref faceRoutine);
        }
    }

    /// <summary> Instantly re-writes the party member UI to match all the information about the pokemon. </summary>
    /// <param name="status"> Status to extract information from. </param>
    public void LoadState(PokemonInstanceStatus status)
    {
        if (statusEffects == null)
            statusEffects = new List<StatusEffectUI>();

        /* Set all UI Elements For This Pokemon */
        portrait_face.gameObject.SetActive(true);
        SetName(status);
        SetFace(status);
        SetLevel(status);
        SetHP(status);
        SetXP(status);
        SetStatusEffects(status);
        isLoaded = true;

    }

    /// <summary> Instantly clears all info about any moves. </summary>
    public void ClearState()
    {
        info_Container.transform.localScale = Vector3.zero;
        portrait_face.gameObject.SetActive(false);
        portrait_Container.transform.localScale = Vector3.one * 0.5f;
        isLoaded = false;
    }

    /// <summary> Clear select states of all but one status effect field. </summary>
    /// <param name="effectUI"> Field to enable. </param>
    public void ClearSelectStates(StatusEffectUI effectUI)
    {
        if (!isLoaded) return;

        foreach (StatusEffectUI field in statusEffects)
        {
            if (field == effectUI)
                field.SetHover(true);

            else
                field.SetHover(false);
        }
    }


    /// <summary> Sets name and gender of pokemon in UI. </summary>
    /// <param name="status"> Pokemon status to extract information from. </param>
    private void SetName(PokemonInstanceStatus status)
    {
        string genderStr = "";

        if (status.state.gender != PokeGender.None)
            genderStr = "  <sprite=" + '"' + status.state.gender.ToString() + '"' + " index=0>";

        name.text = status.Name + genderStr;
    }

    /// <summary> Sets portrait face of pokemon in UI. </summary>
    /// <param name="status"> Pokemon status to extract information from. </param>
    private void SetFace(PokemonInstanceStatus status)
    {
        portrait_face.sprite = status.pokemon.Face;
    }


    /// <summary> Sets level of pokemon in UI.</summary>
    /// <param name="status"> Pokemon status to extract information from. </param>
    private void SetLevel(PokemonInstanceStatus status)
    {
        level.text = "Lvl " + status.state.level;
    }

    /// <summary> Sets HP count and gauge in UI based on pokemon. </summary>
    /// <param name="status"> Pokemon status to extract information from. </param>
    private void SetHP(PokemonInstanceStatus status)
    {
        hpGauge_bar.rectTransform.localScale = new Vector3(status.state.hP, hpGauge_bar.rectTransform.localScale.y, 1);
        hpGauge_bar.color = hpColorPicker.Evaluate(status.state.hP);
    }

    /// <summary> Sets XP gauge based on pokemon. </summary>
    /// <param name="status"> Pokemon status to extract information from. </param>
    private void SetXP(PokemonInstanceStatus status)
    {
        /* Scale and Color the HP Bar */
        xpGauge_bar.rectTransform.localScale = new Vector3(status.state.xP, xpGauge_bar.rectTransform.localScale.y, 1);
    }

    /// <summary> Re-writes the entire status effect display. </summary>
    /// <param name="status"> Pokemon status to extract information from. </param>
    private void SetStatusEffects(PokemonInstanceStatus status)
    {
        /* Remove and Destroy Current Fields */
        for (int i = statusEffects.Count - 1; i >= 0; i--)
        {
            Destroy(statusEffects[i].gameObject);
            statusEffects.RemoveAt(i);
        }

        /* Foreach New Status Effect */
        foreach (StatusEffect effect in status.state.statusEffects)
        {
            /* Instantiate A New Field */
            StatusEffectUI field = Instantiate(statusEffectPrefab, statusEffectsContainer);

            /* Fill In The Field */
            field.Load(effect);

            /* Add This Field To List Of Fields */
            statusEffects.Add(field);

        }
    }

    /// <summary> Changes how choose button is pulsing. </summary>
    /// <param name="state"> True is pulsing and false is no pulse. </param>
    public void HoverChoose(bool state)
    {
        if (!isLoaded) return;

        /* Change Color Pulsing Mode */
        this.StartInterruptableCoroutine(state ?
            chooseButton.FluxColor(Color.white, hpGauge_bar.color, 2) :
            chooseButton.ChangeColorOvertime(Color.white, 8),
            ref chooseRoutine);
    }

    /// <summary> Toggles info state. </summary>
    public void SelectPortrait()
    {
        if (!isLoaded) return;

        if (info_Container.activeInHierarchy)
            HideInfo();

        else
            ShowInfo();

    }

    /// <summary> Shows info about pokemon. </summary>
    public void ShowInfo()
    {
        if (!isLoaded) return;

        /* Play Sound */
        SoundManager.PlaySound("Blip");
        
        /* Scale Up Info */
        this.StartInterruptableCoroutine(ScaleOpen(), ref scaleRoutine);

        /* Brighten Portrait and Scale It Up */
        portrait_Container.color = Color.white;
        this.StartInterruptableCoroutine(
            portrait_Container.transform.ScaleOvertime(Vector3.one * 1.2f, 10f), ref portraitRoutine);
            
        /* Brighten Face */
        this.StartInterruptableCoroutine(
            portrait_face.ChangeColorOvertime(Color.white, 10f), ref faceRoutine);
        
    }

    /// <summary> Hides info about pokemon. </summary>
    public void HideInfo()
    {
        if (!isLoaded) return;

        /* Play Sound */
        SoundManager.PlaySound("Blip");

        /* Scale Info Down */
        this.StartInterruptableCoroutine(ScaleClose(), ref scaleRoutine);

        /* Restore Portrait Size and Start Changing Its Color */
        portrait_Container.transform.localScale = Vector3.one;
        this.StartInterruptableCoroutine(
            portrait_Container.FluxColor(c1, c2, 1), ref portraitRoutine);

        /* Start Changing Faces Color */
        this.StartInterruptableCoroutine(
            portrait_face.FluxColor(c1, c2, 1), ref faceRoutine);

    }

    /// <summary> Activates and scales info container. </summary>
    private IEnumerator ScaleOpen()
    {
        /* Enable Info */
        info_Container.SetActive(true);

        /* Increase Size */
        yield return info_Container.transform.ScaleOvertime(Vector3.one, 16);

    }

    /// <summary> Scales down then deactivates info container. </summary>
    private IEnumerator ScaleClose()
    {
        /* Increase Size */
        yield return info_Container.transform.ScaleOvertime(Vector3.zero, 16);

        info_Container.SetActive(false);

    }
}
