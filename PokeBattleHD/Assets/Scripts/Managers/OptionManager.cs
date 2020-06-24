using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionManager : MonoBehaviour
{
    /* Instance */
    public static OptionManager instance;

    [Tooltip("Reference to pokemon state 1 in UI."), SerializeField]
    private Texture2D cursor = null;

    /* Public References */
    /// <summary> Reference to pokemon state 1 in UI. </summary>
    [Tooltip("Reference to pokemon state 1 in UI.")]
    public PokeUIState state1;

    /// <summary> Reference to pokemon state 2 in UI. </summary>
    [Tooltip("Reference to pokemon state 2 in UI.")]
    public PokeUIState state2;

    /// <summary> Reference to pokeball image. </summary>
    [Tooltip("Reference to pokeball image.")]
    public Image pokeball;

    /// <summary> Reference to fight field in UI. </summary>
    [Tooltip("Reference to fight field in UI.")]
    [SerializeField] private FightUI fight = null;

    /// <summary> Reference to move fight field in UI. </summary>
    [Tooltip("Reference to move fight field in UI.")]
    [SerializeField] private PokemonUI pokemon = null;

    /// <summary> Reference to move fight field in UI. </summary>
    [Tooltip("Reference to move fight field in UI.")]
    [SerializeField] private GameObject bagField = null;

    /// <summary> Reference to move fight field in UI. </summary>
    [Tooltip("Reference to move fight field in UI.")]
    [SerializeField] private GameObject runField = null;

    private IEnumerator rotateRoutine;


    void Awake()
    {
        /* Initialize Instance (Throw Error If More Than One) */
        if (instance != null)
            Debug.LogWarning("More than one Option Manager in this scene.");
        else
            instance = this;

        if (cursor != null)
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);

    }

    /// <summary> Force opens pokemon field and makes all other fields unclickable. </summary>
    public void ForcePokemonSelection()
    {
        /* Enable Options */
        SetOptions(true);

        /* Select Pokemon Field */
        pokemon.Anim.Click();

        /* Make All Main Fields Unclickable To Force Pokemon Selection */
        fight.Anim.Clickable = false;
        pokemon.Anim.Clickable = false;
    }

    /// <summary> Sets pokeball image's direction to face toward target. </summary>
    /// <param name="target"> UI element to face pokeball towards. </param>
    public void SetPokeballDirection(Transform target)
    {
        this.StartInterruptableCoroutine(LookAtTarget(target), ref rotateRoutine);
    }

    /// <summary> Clears direction of pokeball image by making it face up. </summary>
    public void ClearPokeballDirection()
    {
        this.StartInterruptableCoroutine(ClearTarget(), ref rotateRoutine);
    }

    private IEnumerator LookAtTarget(Transform target)
    {
        /* Make The Pokeball White and Increase It's Size */
        pokeball.color = new Color(0.7f, 0.7f, 0.7f, 1);
        pokeball.transform.localScale *= 1.5f;
        
        /* Get Direction From Pokeball To Target */
        Vector3 heading = target.position - pokeball.transform.position;

        /* Gradually Return Pokeball Back To Original Scale and Move It Towards Target */
        while (pokeball.transform.localScale != Vector3.one && pokeball.transform.up != heading) {
            pokeball.transform.up = Vector3.Lerp(pokeball.transform.up, heading, Time.deltaTime * 4f);
            pokeball.transform.localScale = Vector3.Lerp(pokeball.transform.localScale, Vector3.one, Time.deltaTime * 4f);
            yield return null;
        }
    }

    private IEnumerator ClearTarget()
    {
        /* Initialize Target Color */
        Color targetColor = new Color(0.2f, 0.2f, 0.2f, 1);

        /* Gradually Return Pokeball Back To Original Scale and Move It Towards Target */
        while (pokeball.color != targetColor && pokeball.transform.up != Vector3.up)
        {
            pokeball.transform.up = Vector3.Lerp(pokeball.transform.up, Vector3.up, Time.deltaTime * 4f);
            pokeball.color = Color.Lerp(pokeball.color, targetColor, Time.deltaTime * 4f);
            yield return null;
        }
    }

    /// <summary> Load moves from a pokemon. Automatically fills the UI for moves. </summary>
    /// <param name="pokemon"> Pokemon to get moves from. </param>
    public void LoadMoves(Pokemon pokemon)
    {
        fight.LoadMoves(pokemon);
    }

    /// <summary> Load party from a trainer. Automatically fills the UI for party. </summary>
    /// <param name="trainer"> Trainer to get party from. </param>
    public void LoadParty(Trainer trainer)
    {
        pokemon.LoadParty(trainer);
    }

    /// <summary> Enable/disable player options UI. </summary>
    /// <param name="state"> Determines whether to disable or enable options.</param>
    public void SetOptions(bool state)
    {
        /* Disable Move Data / Status Effect Data Panels If False */
        if (state == false) {
            StatusEffectPanel.instance.gameObject.SetActive(false);
            MovePanel.instance.gameObject.SetActive(false);
        }
            
        /* Set Activity Of All Fields */
        fight.gameObject.SetActive(state);
        pokemon.gameObject.SetActive(state);
        bagField.SetActive(state);
        runField.SetActive(state);
        pokeball.gameObject.SetActive(state);

        
    }

    /// <summary> Enable/disable pokemon states in the UI. </summary>
    /// <param name="state"> Determines whether to disable or enable options.</param>
    public void SetPokeStates(bool state)
    {
        state1.gameObject.SetActive(state);
        state2.gameObject.SetActive(state);
    }

}

   
