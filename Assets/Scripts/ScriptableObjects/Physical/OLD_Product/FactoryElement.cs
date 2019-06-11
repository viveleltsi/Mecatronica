using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A factory is a building producing some stuff with a production blueprint
/// </summary>
[CreateAssetMenu(fileName = "factory", menuName = "Data/Building/Factory")]
public class FactoryElement : BuildingElement
{
    /// <summary>
    /// receipes for producing good
    /// </summary>
    public List<ProductionBlueprint> ProductBlueprints = new List<ProductionBlueprint>();

    /// <summary>
    /// Loss of the factory
    /// </summary>
    public LossProfile Loss;
}

[Serializable]
public class LossProfile
{
    public float EnergyLoss;
    public float PhysicalLoss;
    public List<SpecificLooseElement> SpecificLoss = new List<SpecificLooseElement>();
}

[Serializable]
public class SpecificLooseElement
{
    public AbstractScriptableObjectElement Element;
    public float Loose;
}
