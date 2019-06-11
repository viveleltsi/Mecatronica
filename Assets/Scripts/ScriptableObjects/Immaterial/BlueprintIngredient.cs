using System;
using UnityEngine;

[Serializable]
public class BlueprintIngredient
{
    /// <summary>
    /// Component
    /// </summary>
    [LinkToScriptableObject(typeof(AbstractTransportableElement), "Transportable resource")]
    public AbstractTransportableElement Component;

    /// <summary>
    /// Quantity (in kg or m^3) for 1 result (kg or m^3)
    /// </summary>
    [Header("Quantity without the loss")]
    public float Quantity;
}


