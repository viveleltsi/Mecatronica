    using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "container", menuName = "Data/Product/Container")]
public class ContainerElement : ProductElement
{
    /// <summary>
    /// Types allowed inside this container
    /// </summary>
    public List<PhysicalState> Types = new List<PhysicalState>();

    /// <summary>
    /// Maximum weight available on this container
    /// </summary>
    public float MaxWeight;

    /// <summary>
    /// Maximum volume available on this container
    /// </summary>
    public float MaxVolume;
}
