using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;

public abstract class Panel : MonoBehaviour
{
    [Tooltip("Field prefab to use when instancing new fields."), SerializeField]
    private PanelField fieldPrefab = null;

    [Tooltip("Name text to update for moves."), SerializeField]
    private TextMeshProUGUI nameText = null;

    [Tooltip("Description text to update for moves."), SerializeField]
    private TextMeshProUGUI description = null;

    private List<PanelField> fields;
    private bool isDragging;
    private IEnumerator scaleRoutine;

    /// <summary> True if panel is open and false if its closing or closed. </summary>
    protected bool isObserving;

    protected virtual void Awake()
    {
        /* Initialze Private Properties */
        fields = new List<PanelField>();
        isDragging = false;

        /* Set Observation State */
        isObserving = true;

        /* Disable Itself */
        gameObject.SetActive(false);
    }

    /// <summary> 
    /// Fills panel with instances of the designated field prefab.
    /// Each field will be filled with information from the list of effect descriptions.
    /// The description will be placed on the bottom of the panel after all the effect descriptions.
    /// </summary>
    /// <param name="_name"> Name of data-bearing object. </param>
    /// <param name="_effectDescriptions"> List of effect descriptions. </param>
    /// <param name="_description"> Description for bottom of panel. </param>
    protected void FillPanel(string _name, EffectDescription[] _effectDescriptions, string _description)
    {
        /* Fill In Name */
        nameText.text = _name;

        /* Remove and Destroy Current Fields */
        for (int i = fields.Count - 1; i >= 0; i--)
        {
            Destroy(fields[i].gameObject);
            fields.RemoveAt(i);
        }

        /* Fill In Description Of Data */
        description.text = _description;

        /* For Each Effect In List  */
        foreach(EffectDescription effectDescription in _effectDescriptions)
        {
            /* Instantiate A New Field */
            PanelField field = Instantiate(fieldPrefab, transform);

            /* Fill In The Field */
            field.FillField(effectDescription);

            /* Add This Field To List Of Fields */
            fields.Add(field);
        }

        /* Place Description As Last Child So That It Is Placed On The Bottom Of The Layout Group */
        description.rectTransform.SetAsLastSibling();
    }

    /// <summary> Hides all current fields' tooltips. </summary>
    public void HideAllFieldTooltips()
    {
        foreach (PanelField field in fields)
            field.SetTooltip(false);
    }

    /// <summary> Sets visibility of data panel. </summary>
    /// <param name="state"> True for visible, false for invisible. </param>
    public void SetVisibility(bool state)
    {
        /* If True and Panel Is NOT Active, Enable The Panel Then Scale It Up */
        if (state == true) {
            gameObject.SetActive(true);
            this.StartInterruptableCoroutine(ScaleUp(), ref scaleRoutine);
        }
            

        /* Else (False), If Panel Is Active, Scale The Panel Down Then Disable It */
        else if (gameObject.activeInHierarchy)
            this.StartInterruptableCoroutine(ScaleDown(), ref scaleRoutine);

    }

    /// <summary> Toggles draggability of panel. </summary>
    public void ToggleDrag()
    {
        /* Update Drag State */
        isDragging = !isDragging;

        /* Start Dragging If State Is True */
        if (isDragging)
            StartCoroutine(FollowPointer());

    }

    private IEnumerator FollowPointer()
    {
        /* Get Initial Offset Between Panel and Mouse */
        Vector3 offset = transform.position - Input.mousePosition;

        /* Move Panel as Long As Dragging Is Required */
        while (isDragging) {
            transform.position = offset + Input.mousePosition;
            yield return null;
        }
    }

    /// <summary> Makes panel small then enlarges it over time. </summary>
    private IEnumerator ScaleUp()
    {
        /* Set Observation State */
        isObserving = true;

        /* Decrease Panel Size */
        transform.localScale = Vector3.zero;

        /* Decrease Size */
        yield return transform.ScaleOvertime(Vector3.one, 20);
    }

    /// <summary> Makes panel small then enlarges it over time. </summary>
    private IEnumerator ScaleDown()
    {
        /* Set Observation State */
        isObserving = false;

        /* Decrease Size */
        yield return transform.ScaleOvertime(Vector3.zero, 20);

        /* Disable Panel */
        gameObject.SetActive(false);

    }
}
