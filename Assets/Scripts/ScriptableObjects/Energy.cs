using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "energy", menuName = "Data/Element/Energy")]
public class Energy : AbstractScriptableObjectElement
{
    
}

public enum EnergyKind
{
    Heat,
    Electricity,
}
