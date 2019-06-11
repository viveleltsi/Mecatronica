using System.Collections;
using UnityEngine;

/// <summary>
/// Product is an object that can't be subdivide
/// It has a shape, mass, volume
/// </summary>
[CreateAssetMenu(fileName = "product", menuName = "Data/Product/Product")]
public class ProductElement : MixedPhysicalElement
{
    /// <summary>
    /// Mass of the product
    /// </summary>
    public float Mass;

    /// <summary>
    /// Dimension when on the ground -> plan x,z horizontal. y is vertical axe
    /// Unit is meter [m]
    /// </summary>
    public Vector3 Dimension;

    /// <summary>
    /// Volume in m^3 [m^3]
    /// </summary>
    public float Volume
    {
        get
        {
            return Dimension.x * Dimension.y * Dimension.z;
        }
    }
}
