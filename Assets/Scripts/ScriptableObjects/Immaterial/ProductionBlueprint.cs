using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "productionBlueprint", menuName = "Data/BluePrint/Production")]
public class ProductionBlueprint : AbstractBlueprint
{
    /// <summary>
    /// Result of the production step
    /// </summary>
    [Space(10)]
    public List<BlueprintResult> Results = new List<BlueprintResult>();

    /// <summary>
    /// Quantity to make on each step
    /// </summary>
    [Space(10)]
    public float QuantityToMake;

    /// <summary>
    /// Fabrication Time of the product in second
    /// </summary>
    public float FabricationTime;
}