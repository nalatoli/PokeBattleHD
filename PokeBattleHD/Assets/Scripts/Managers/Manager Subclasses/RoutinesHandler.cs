using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary> Handles events that should be invoked in a prioritized order. </summary>
public class RoutinesHandler
{
    /// <summary> Delegates to invoke when this event is raised. </summary>
    private event Func<IEnumerator> OnEvent;

    /// <summary> Map of priority values to delegates for reordering. </summary>
    private readonly Dictionary<int, Func<IEnumerator>> priorities;

    /// <summary> Generates an event handler with a priority range of 2->1->0. </summary>
    public RoutinesHandler()
    {
        OnEvent = null;
        priorities = new Dictionary<int, Func<IEnumerator>>();
    }

    /// <summary> Subscribes a function with specified priority. </summary>
    /// <param name="routine"> Function to subscribe. </param>
    /// <param name="priority"> Priority level of function from 2->1->0. </param>
    public void Subscribe(Func<IEnumerator> routine, EventPriorityLevel priority)
    {
        /* Throw Error At User and Return If Event Priority Map Already Contains A Priority For This Routine */
        if (priorities.ContainsValue(routine)) {
            Debug.LogError(routine + " is already subscribed to " + OnEvent);
            return;
        }

        /* Add This Routine To Event Priority Map */
        priorities.Add((int)priority, routine);

        /* Reorder Events */
        Reorder();

    }

    /// <summary> Unsubscribes specified function. </summary>
    /// <param name="routine"> Function to unsubscribe. </param>
    public void Unsubscribe(Func<IEnumerator> routine)
    {
        /* Throw Error At User And Return If Routine Is NOT Subscribed */
        if (!priorities.ContainsValue(routine))
        {
            Debug.Log("Can't unsubscribe an event that wasn't subscribed");
            return;
        }

        /* Remove The Routine From The Dictionary and Unsubscribe Event */
        priorities.Remove(priorities.First(kvp => kvp.Value == routine).Key);

        /* Reorder Events */
        Reorder();

    }

    /// <summary> Sequentially peform events in prioritized order. </summary>
    public IEnumerator Raise()
    {
        /* If There Are NO Subscribers To This Event, Return Immedietly */
        if (OnEvent == null)
            yield break;

        /* Call Each Routine In Invocation List One At A Time */
        Delegate[] routines = OnEvent.GetInvocationList();
        foreach (Delegate routine in routines)
            yield return routine.DynamicInvoke();

    }

    /// <summary> Reorders invocation list of event. </summary>
    private void Reorder()
    {
        /* Initialize List Of Pending Subscribers */
        List<Func<IEnumerator>> routines = new List<Func<IEnumerator>>();

        /* Fill List Of Pending Subscribers Depending On Priority */
        for (int i = 2; i >= 0; i--)
            if (priorities.ContainsKey(i))
                routines.AddRange(priorities.Keys.Where(k => k == i).Select(k => priorities[k]));

        /* Clear Exsisting Subscribers, Then Add Pending Subscribers */
        OnEvent = null;
        foreach (Func<IEnumerator> routine in routines)
            OnEvent += routine;

    }
}

/// <summary> 
/// Priority level of subscribe that determines when a
/// subscriber will perform within an event call. 
/// </summary>
public enum EventPriorityLevel
{
    /// <summary>
    /// These routines go last after the other 
    /// two priority levels. This priority level is made for 
    /// effects that react to reactive effects (level 1) effects.
    /// </summary>
    Level0,

    /// <summary>
    /// These routines go after level 2 priority routines. 
    /// This priority level is made for effects that react 
    /// to non-reactive (level 2) effects.
    /// </summary>
    Level1,

    /// <summary>
    /// These routines will always go first. 
    /// This priority level is made for non-reactive 
    /// effects (effects that do not rely on other effects).
    /// </summary>
    Level2

}
