using Dan.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Any element who exist, atom and stuff
/// </summary>
public abstract class AbstractPhysicalElement : AbstractScriptableObjectElement{
    /// <summary>
    /// State of the physical element
    /// </summary>
    public PhysicalState State;

    /// <summary>
    /// Prefab descriptor to get a prefab link
    /// </summary>
    [LinkToScriptableObject(typeof(PrefabDescriptor), "Prefab link")]
    public string PrefabDescriptorId;
}
