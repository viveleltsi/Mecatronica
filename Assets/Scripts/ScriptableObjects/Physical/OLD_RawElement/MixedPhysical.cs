using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Physical element composed by more than one component
/// </summary>
[CreateAssetMenu(fileName = "mixed", menuName = "Data/Element/Mixed")]
public class MixedPhysicalElement : AbstractPhysicalElement
{
    /// <summary>
    /// Volumic mass [kg/m^3]
    /// </summary>
    public short Density;
    
    /// <summary>
    /// Composition of the allow
    /// </summary>
    public List<Composition> Composition = new List<Composition>();
}

[Serializable]
public struct Composition
{
    /// <summary>
    /// Massic percent of the element
    /// from 0 to 1
    /// </summary>
    public float Percent;

    /// <summary>
    /// Element
    /// </summary>
    public AbstractScriptableObjectElement Element;
}


