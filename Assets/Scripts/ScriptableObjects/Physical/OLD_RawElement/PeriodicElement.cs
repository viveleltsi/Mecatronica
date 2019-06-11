using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "periodicElement", menuName = "Data/Element/Periodic")]
public class PeriodicElement : AbstractScriptableObjectElement
{
    /// <summary>
    /// Atomic number
    /// </summary>
    public short AtomicNumber;
    /// <summary>
    /// Mass volumic [kg/m^3]
    /// </summary>
    public short Density;
    /// <summary>
    ///  Temperature where the fusion occure in Kelvin
    /// </summary>
    public short MeltingTemperature;
    /// <summary>
    /// Temperature where the vaporization occur in kelvin
    /// </summary>
    public short VaporizationTemperature;
}
