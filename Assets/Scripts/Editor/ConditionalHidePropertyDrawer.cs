using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }

    private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        bool enabled = true;
        string propertyPath = property.propertyPath;
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField);
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

        if (sourcePropertyValue != null)
        {
            enabled = CheckPropertyValue(sourcePropertyValue, condHAtt.CompareValue);
        }
        else
        {
            Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }

        return enabled;
    }

    private bool CheckPropertyValue(SerializedProperty sourcePropertyValue, object compareValue)
    {
        if (sourcePropertyValue.propertyType == SerializedPropertyType.Enum)
        {
            return sourcePropertyValue.enumValueIndex.Equals((int)compareValue);
        }
        else if (sourcePropertyValue.propertyType == SerializedPropertyType.Boolean)
        {
            return sourcePropertyValue.boolValue.Equals(compareValue);
        }
        else if (sourcePropertyValue.propertyType == SerializedPropertyType.Integer)
        {
            return sourcePropertyValue.intValue.Equals(compareValue);
        }
        else if (sourcePropertyValue.propertyType == SerializedPropertyType.Float)
        {
            return sourcePropertyValue.floatValue.Equals(compareValue);
        }
        else if (sourcePropertyValue.propertyType == SerializedPropertyType.String)
        {
            return sourcePropertyValue.stringValue.Equals(compareValue);
        }

        return true;
    }
}
