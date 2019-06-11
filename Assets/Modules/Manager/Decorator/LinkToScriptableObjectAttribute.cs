using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkToScriptableObjectAttribute : PropertyAttribute
{
    public Type Type;

    public string CustomLabel;

    public LinkToScriptableObjectAttribute(Type type,string customLabel = "")
    {
        Type = type;
        CustomLabel = customLabel;
    }
}
