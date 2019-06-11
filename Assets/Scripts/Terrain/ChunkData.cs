using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Data of a chunk
/// </summary>
public class ChunkData
{
    /// <summary>
    /// Dictionary for data, level of detail is the key
    /// </summary>
    public Dictionary<int, MeshData> Datas = new Dictionary<int, MeshData>();

    /// <summary>
    /// Dictionary for mesh, level of detail is the key
    /// </summary>
    public Dictionary<int, Mesh> Meshs = new Dictionary<int, Mesh>();

    /// <summary>
    /// Link to the gameobject who have the MeshRenderer
    /// </summary>
    public GameObject GameObject;

    /// <summary>
    /// Current level of details
    /// </summary>
    public int CurrentLod = -1;

    /// <summary>
    /// Position of iron meteoric positions (temporary solutions for resources)
    /// </summary>
    public List<Vector2> MeteoricIronPositions = new List<Vector2>();

    /// <summary>
    /// All iron meteoric gameobjects (temporary for the "only one resource" version)
    /// </summary>
    public List<GameObject> MeteoricIronGameobject = new List<GameObject>();
}
