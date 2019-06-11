using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBlueprint : AbstractImmaterialElement
{
    /// <summary>
    /// Ingredient of the production step
    /// </summary>
    [Space(20)]
    public List<BlueprintIngredient> Ingredients = new List<BlueprintIngredient>();
}


