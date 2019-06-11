using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "building", menuName = "Data/Building/Building")]
public class BuildingElement : ProductElement
{
    /// <summary>
    /// Size of the grid
    /// </summary>
    public Vector2 GridSize;

    /// <summary>
    /// All special zone of the building
    /// </summary>
    public List<BuildingZone> Zones = new List<BuildingZone>();
}

[Serializable]
public class BuildingZone
{
    public Vector2Int Position;
    public BuildingZoneKind Kind;
}

public enum BuildingZoneKind
{
    In,
    Out,
    Stock,
    Moving
}


