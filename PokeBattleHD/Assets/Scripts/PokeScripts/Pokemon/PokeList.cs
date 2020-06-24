using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// Unique list of pokemon types  
/// that calls the holder-pokemon's 'TypeChanged' event when changed.
/// </summary>
[System.Serializable]
public class PokeTypeList
{
    [Tooltip("List of status effects for this pokemon."), SerializeField]
    private List<PokeType> list;

    /// <summary> Pokemon to call event from. </summary>
    protected Pokemon pokemon;

    /// <summary> Generate new status effect list that calls 'StatusEffectChanged' event when changed. </summary>
    /// <param name="pokemon"> Pokemon to call event from. </param>
    public PokeTypeList(Pokemon pokemon)
    {
        this.pokemon = pokemon;
        list = new List<PokeType>();
    }

    /// <summary> Get/set unique pokemon type at specified index. Will NOT set if type is not unique. </summary>
    /// <param name="i"> Index of pokemon type. </param>
    /// <returns> Pokemon type at specified index. </returns>
    public PokeType this[int i]
    {
        get { return list[i]; }
        set {
            if (list.Contains(value)) return;
            list[i] = value;
            pokemon.Events.TypeChanged();
        }
    }

    /// <summary> 
    /// Adds a pokemon type to the list if the pokemon type is not in it already. 
    /// If pokemon type is added succesfully, the pokemon's 'TypeChanged' event will be called. 
    /// </summary>
    /// <param name="item"> Unique pokemon type to add. </param>
    /// <returns> True if pokemon type is added successfully (is unique), false otherwise. </returns>
    public bool Add(PokeType item)
    {
        if (list.Contains(item)) return false;
        list.Add(item);
        pokemon.Events.TypeChanged();
        return true;    
    }

    /// <summary> 
    /// Removes unique pokemon type from list if the pokemon type is in the list. 
    /// If the pokemon type is removed succesfully, the pokemon's 'TypeChanged' event will be called.  
    /// </summary>
    /// <param name="item"> Unique pokemon type to remove. </param>
    /// <returns> True if the pokemon type is removed successfully, false otherwise. </returns>
    public bool Remove(PokeType item)
    {
        if (!list.Remove(item)) return false;
        pokemon.Events.TypeChanged();
        return true;
    }

    /// <summary> Determines if pokemon type is in this list. </summary>
    /// <param name="item"> Pokemon type to look for. </param>
    /// <returns> True if in the list, false otherwise. </returns>
    public bool Contains(PokeType item) => list.Contains(item);

    /// <summary> Enumerator for pokemon type list. </summary>
    public IEnumerator GetEnumerator() => list.GetEnumerator();

    /// <summary> Clears list of pokemon types. If list was actually changed, the pokemon's 'TypeChanged' event will be called.</summary>
    public void Clear() { if (list.Count >= 1) list.Clear(); pokemon.Events.TypeChanged(); }

    /// <summary> Gets the index of a pokemon type in this list. </summary>
    /// <param name="item"> Pokemon type to look for. </param>
    /// <returns> Index of the pokemon type. </returns>
    public int IndexOf(PokeType item) => list.IndexOf(item);

    /// <summary> 
    /// Inserts pokemon type into specified index if the pokemon type is unique.
    /// Items on and past this index will be pushed one index up. </summary>
    /// If successful, the pokemon's 'TypeChanged' event will be called.
    /// <param name="index"> Index to insert into. </param>
    /// <param name="item"> Pokemon type to insert. </param>
    /// <returns> True if inserted successfully, false otherwise. </returns>
    public bool Insert(int index, PokeType item)
    {
        if (list.Contains(item)) return false;
        list.Insert(index, item);
        pokemon.Events.TypeChanged();
        return true;
    }

    /// <summary> 
    /// Removes pokemon type from specified index.
    /// Items past this index will be pulled one index down. </summary>
    /// The pokemon's 'TypeChanged' event will be called.
    /// <param name="index"> Index to remove from. </param>
    public void RemoveAt(int index) { list.RemoveAt(index); pokemon.Events.TypeChanged(); }

    /// <summary> Gets number of pokemon types in this list. </summary>
    public int Count { get { return list.Count; } }

}

/// <summary> 
/// Unique list of status effects  
/// that calls the holder-pokemon's 'StatusEffectChanged' event when changed.
/// </summary>
public class StatusEffectList
{
    /// <summary> Reference to original list filled with prefabs. </summary>
    private List<StatusEffect> prefabList;

    /// <summary> New list that is filled with instantiated versions of the original list's prefabs. </summary>
    private List<StatusEffect> instanceList;

    /// <summary> Parent of new status effect instances. </summary>
    private Transform parent;

    /// <summary> Pokemon to call event from. </summary>
    protected Pokemon pokemon;

    /// <summary> Returns an instantiated, parented, and renamed version of a prefab. </summary>
    /// <param name="prefab"> Status effect prefab to clone. </param>
    /// <returns> Renamed status effect instance. </returns>
    private StatusEffect Generate(StatusEffect prefab)
    {
        StatusEffect instance = (parent == null) ? 
            Object.Instantiate(prefab) : Object.Instantiate(prefab, parent);
        instance.name = prefab.name;
        return instance;
    }

    /// <summary> Removes prefab from prefab list. Destroys and removes instance from instance list. </summary>
    /// <param name="prefab"> Prefab to look for in prefab list. </param>
    private void SmartRemove(StatusEffect prefab)
    {
        int i = prefabList.IndexOf(prefab);
        Object.Destroy(instanceList[i].gameObject);
        instanceList.RemoveAt(i);
        prefabList.RemoveAt(i);
    }

    /// <summary> Removes prefab from specified index from prefab list. Destroys and removes instance from instance list. </summary>
    /// <param name="index"> Index to remove from. </param>
    private void SmartRemove(int index)
    {
        Object.Destroy(instanceList[index].gameObject);
        instanceList.RemoveAt(index);
        prefabList.RemoveAt(index);
    }


    /// <summary> 
    /// Create new status effect instances from a status effect prefab list.
    /// The specified pokemon will call its 'StatusEffectChanged' Event whenever this list is updated.
    /// Warning: Instance transform properties will be randomly assigned. Use parent overload to assign a parent.
    /// </summary>
    /// <param name="pokemon"> Pokemon to call event from. </param>
    /// <param name="prefabList"> Prefab list to get status effect prefabs from. </param>
    public StatusEffectList(Pokemon pokemon, List<StatusEffect> prefabList)
    {
        /* Initialize Properties */
        this.pokemon = pokemon;
        this.prefabList = prefabList;
        parent = null;
        instanceList = new List<StatusEffect>();
        
        /* Generate All Prefabs From Prefab List And Add To Instance List */
        foreach(StatusEffect effect in prefabList)
            instanceList.Add(Generate(effect));
        
    }

    /// <summary> 
    /// Create new status effect instances from a status effect prefab list.
    /// The specified pokemon will call its 'StatusEffectChanged' Event whenever this list is updated.
    /// </summary>
    /// <param name="pokemon"> Pokemon to call event from. </param>
    /// <param name="prefabList"> Prefab list to get status effect prefabs from. </param>
    /// <param name="parent"> Parent of new status effect instances. </param>
    public StatusEffectList(Pokemon pokemon, List<StatusEffect> prefabList, Transform parent)
    {
        /* Initialize Properties */
        this.pokemon = pokemon;
        this.prefabList = prefabList;
        this.parent = parent;
        instanceList = new List<StatusEffect>();

        /* Generate All Prefabs From Prefab List And Add To Instance List */
        foreach (StatusEffect effect in prefabList)
            instanceList.Add(Generate(effect));
    }
    
    /// <summary> Get/set unique status effect at specified index. Will NOT set if effect is not unique. </summary>
    /// <param name="i"> Index of status effect. </param>
    /// <returns> Status effect at specified index. </returns>
    public StatusEffect this[int i]
    {
        get { return instanceList[i]; }
        set {
            if (prefabList.Contains(value)) return;
            prefabList[i] = value;
            instanceList[i] = Generate(value);
            pokemon.Events.StatusEffectChanged();
        }
    }

    /// <summary> 
    /// Adds a status effect to the list if the status effect is not in it already. 
    /// If status effect is added succesfully, the pokemon's 'StatusEffectChanged' event will be called. 
    /// </summary>
    /// <param name="item"> Unique status effect to add. </param>
    /// <returns> True if status effect is added successfully (is unique), false otherwise. </returns>
    public bool Add(StatusEffect item)
    {
        if (prefabList.Contains(item)) return false;
        prefabList.Add(item);
        instanceList.Add(Generate(item));
        pokemon.Events.StatusEffectChanged();
        return true;    
    }

    /// <summary> 
    /// Removes unique status effect from list if the status effect is in the list. 
    /// If the status effect is removed succesfully, the pokemon's 'StatusEffectChanged' event will be called.  
    /// </summary>
    /// <param name="item"> Unique status effect to remove. </param>
    /// <returns> True if the status effect is removed successfully, false otherwise. </returns>
    public bool Remove(StatusEffect item)
    {
        if (!prefabList.Contains(item)) return false;
        SmartRemove(item);
        pokemon.Events.StatusEffectChanged();
        return true;
    }

    /// <summary> Determines if status effect is in this list. </summary>
    /// <param name="item"> status effect to look for. </param>
    /// <returns> True if in the list, false otherwise. </returns>
    public bool Contains(StatusEffect item) => prefabList.Contains(item);

    /// <summary> Enumerator for status effect list. </summary>
    public IEnumerator GetEnumerator() => instanceList.GetEnumerator();

    /// <summary> Clears list of status effects. If list was actually changed, the pokemon's 'StatusEffectChanged' event will be called.</summary>
    public void Clear()
    {
        if (prefabList.Count < 1) return;
        for (int i = prefabList.Count - 1; i >= 0; i--)
            SmartRemove(i);
        pokemon.Events.StatusEffectChanged();
    }

    /// <summary> Gets the index of a status effect in this list. </summary>
    /// <param name="item"> Status effect to look for. </param>
    /// <returns> Index of the status effect. </returns>
    public int IndexOf(StatusEffect item) => prefabList.IndexOf(item);

    /// <summary> 
    /// Inserts status effect into specified index if the status effect is unique.
    /// Items on and past this index will be pushed one index up. </summary>
    /// If successful, the pokemon's 'StatusEffectChanged' event will be called.
    /// <param name="index"> Index to insert into. </param>
    /// <param name="item"> Status effect to insert. </param>
    /// <returns> True if inserted successfully, false otherwise. </returns>
    public bool Insert(int index, StatusEffect item)
    {
        if (prefabList.Contains(item)) return false;
        prefabList.Insert(index, item);
        instanceList.Insert(index, Generate(item));
        pokemon.Events.StatusEffectChanged();
        return true;
    }

    /// <summary> 
    /// Removes status effect from specified index.
    /// Items past this index will be pulled one index down. </summary>
    /// The pokemon's 'StatusEffectChanged' event will be called.
    /// <param name="index"> Index to remove from. </param>
    public void RemoveAt(int index) 
    {
        if (index < 0 || index >= prefabList.Count) return;
        SmartRemove(index);
        pokemon.Events.StatusEffectChanged();
    }

    /// <summary> Gets number of status effects in this list. </summary>
    public int Count { get { return prefabList.Count; } }

}
