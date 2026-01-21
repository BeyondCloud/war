using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ConditionalHideAttribute : PropertyAttribute
{
    public string ConditionalSourceField;
    public object CompareValue;

    public ConditionalHideAttribute(string conditionalSourceField, object compareValue)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.CompareValue = compareValue;
    }
}
