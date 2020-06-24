using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        /*Get Attribute Data */
        ConditionalHideAttribute cha = (ConditionalHideAttribute)attribute;

        /* Get if Property is True or False */
        bool enabled = GetConditionalHideAttributeResult(cha, property);

        /* Draw the Property If NOT Hidden */
        if (cha.hideInInspector != enabled)
        {
            if (cha.min == cha.max)
                EditorGUI.PropertyField(position, property, label, true);
            else
                EditorGUI.Slider(position, property, cha.min, cha.max);

        }
        
    }

    private static bool HasAttr<T>(SerializedProperty property) where T : Attribute
    {
        Type parentType = property.serializedObject.targetObject.GetType();
        FieldInfo fi = parentType.GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        // fi is sometimes null for reasons I ignore but it still seems to work
        return fi?.GetCustomAttribute<T>() != null;
    }


    /// <summary> Gets inspector value of sourcefield boolean based off the conditional hide attribute. </summary>
    /// <param name="_cha"> The conditional hide attribute. </param>
    /// <param name="_property"> The inspector boolean. </param>
    /// <returns></returns>
    private bool GetConditionalHideAttributeResult(ConditionalHideAttribute _cha, SerializedProperty _property)
    {
        /* Get Inspector Property Path For Condtion Field */
        string conditionPath = _property.propertyPath.Replace(_property.name, _cha.sourceField);

        /* Get Condition Property from Path */
        SerializedProperty inspectorConditionProperty = _property.serializedObject.FindProperty(conditionPath);

        /* If Property Is In Inspector (Exsists) */
        if (inspectorConditionProperty != null)
        {
            /* Return Boolean Value if Property is A Boolean */
            if(inspectorConditionProperty.propertyType == SerializedPropertyType.Boolean)
                return inspectorConditionProperty.boolValue;

            /* Othewise, Return Comparision Result of Property Enum Index and Value if Property is An Enumeration */
            if (inspectorConditionProperty.propertyType == SerializedPropertyType.Enum)
                return _cha.values.Contains(inspectorConditionProperty.enumValueIndex);

            /* Otherwsie, Warn User That Property is NOT A Valid Type And Return Default */
            Debug.LogWarning("SourceProprty not a valid type to be used with ConditionalHideAttribute. Only booleans and enumerations are allowed.");
            return true;
        }
            

        /* Otherise, Warn User That Proprty Is NOT in Inspector And Return Default */
        Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + _cha.sourceField);
        return true;

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        /* Get Attribute Data */
        ConditionalHideAttribute cha = (ConditionalHideAttribute)attribute;

        /* Get if Property is True or False */
        bool enabled = GetConditionalHideAttributeResult(cha, property);

        /* Return Property Height of Property if NOT Hidden */
        if (cha.hideInInspector != enabled)
            return EditorGUI.GetPropertyHeight(property, label);

        /* Else, Return the Default Vertical Spacing */
        else
            return -EditorGUIUtility.standardVerticalSpacing;
    }
}
