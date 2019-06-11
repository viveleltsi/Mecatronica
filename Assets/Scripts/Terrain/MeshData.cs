using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Data of a mesh
/// </summary>
public class MeshData
{

    #region threaded

    private volatile bool _isTaskDone = false;

    public bool IsTaskDone { get { return _isTaskDone; } }
    #endregion

    /// <summary>
    /// All vertices
    /// </summary>
    public List<Vector3> Vertices = new List<Vector3>();

    /// <summary>
    /// All triangles
    /// </summary>
    public List<int> Triangles = new List<int>();

    /// <summary>
    /// All colors (debug purpose)
    /// </summary>
    public List<Color> Colors = new List<Color>();

    /// <summary>
    /// All normals
    /// </summary>
    public List<Vector3> Normals = new List<Vector3>();

    /// <summary>
    /// Link to the terrain generator
    /// </summary>
    private TerrainGenerator _terrainGenerator;

    /// <summary>
    /// Current color to use (for debug purpose)
    /// </summary>
    public Color CurrentColor;

    /// <summary>
    /// Level of detail of this data
    /// </summary>
    public int _lod = -1;

    /// <summary>
    /// Position of the chunk
    /// </summary>
    public Vector2Int ChunkPosition;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="terrainGenerator">Link to the terrain generator used</param>
    /// <param name="lod">Level of details</param>
    /// <param name="chunkPosition"> Position of the chunk</param>
    public MeshData(TerrainGenerator terrainGenerator, int lod, Vector2Int chunkPosition) {
        CurrentColor = Color.gray;
        _terrainGenerator = terrainGenerator;
        _lod = lod;
        ChunkPosition = chunkPosition;
    }

    /// <summary>
    /// Get a grid mesh with step mode
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="terrainGenerator"></param>
    /// <returns></returns>
    public Mesh GetGridMeshStep(Vector3[,] grid)
    {
        GenerateMeshStepData(grid, true);
        return MeshData.GetMesh(this);
    }

    /// <summary>
    /// Start the generation of the mesh with thread
    /// </summary>
    /// <param name="gridObject"></param>
    public void StartGenerateMeshThreaded(object gridObject)
    {
        Vector3[,] grid = (Vector3[,])gridObject;
        _isTaskDone = false;
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        GenerateMeshStepData(grid, true);
        _isTaskDone = true;
    }

    /// <summary>
    /// Intern grid mesh constructor
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="terrainGenerator"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    private void GenerateMeshStepData(Vector3[,] grid, bool step = false)
    {
        List<Vector2Int> quadNotDrawList = new List<Vector2Int>();  // Quad not draw for merging
        for (int y = 0; y < grid.GetLength(0) - 1; y++)
        {
            for (int x = 0; x < grid.GetLength(1) - 1; x++)
            {
                var elevation = 1f;// _terrainGenerator.StepHeight;
                var p1 = GenerateElevation(grid[x, y], elevation);
                var p2 = GenerateElevation(grid[x + 1, y], elevation);
                var p3 = GenerateElevation(grid[x + 1, y + 1], elevation);
                var p4 = GenerateElevation(grid[x, y + 1], elevation);
                if (step)
                {
                    var quadPosition = new Vector2Int(x, y);
                    var quadNotDraw = this.CreateSquareStep(new List<Vector3> { p1, p2, p3, p4 }, quadPosition);
                    if(quadNotDraw == false)
                    {
                        quadNotDrawList.Add(quadPosition);
                    }
                    if(quadNotDrawList.Count > 0 && (quadNotDraw || (x + 1 == grid.GetLength(1) - 1)))
                    {
                        //Quad was draw, we need to merge actual quad
                        var minQuad = new Vector2Int(quadNotDrawList.Min(p => p.x), quadNotDrawList.Min(p => p.y));
                        var maxQuad = new Vector2Int(quadNotDrawList.Max(p => p.x), quadNotDrawList.Max(p => p.y));
                        p1 = GenerateElevation(grid[minQuad.x, minQuad.y], elevation);
                        p2 = GenerateElevation(grid[maxQuad.x + 1, minQuad.y], elevation);
                        p3 = GenerateElevation(grid[maxQuad.x + 1, maxQuad.y + 1], elevation);
                        p4 = GenerateElevation(grid[minQuad.x, maxQuad.y + 1], elevation);
                        var points = new List<Vector3> { p1, p2, p3, p4 };
                        this.CreateQuad(points, Vector3.up, points.Min(p => p.y));
                        quadNotDrawList.Clear();
                    }
                }
                else
                    this.CreateQuad(p1, p2, p3, p4);
            }
        }
    }

    /// <summary>
    /// Get the mesh with current data
    /// </summary>
    /// <returns></returns>
    public static Mesh GetMesh(MeshData data)
    {
        Mesh mesh = new Mesh();
        mesh.SetVertices(data.Vertices);
        mesh.SetTriangles(data.Triangles, 0);
        mesh.SetColors(data.Colors);
        mesh.RecalculateNormals();
        return mesh;
    }

    /// <summary>
    /// Generate the elevation
    /// Method to create "terace"
    /// </summary>
    /// <param name="vector">Position</param>
    /// <param name="stepHeight">Height of a step</param>
    /// <returns>Position with corrected height</returns>
    public Vector3 GenerateElevation(Vector3 vector, float stepHeight)
    {
        float chunkRealSize = _terrainGenerator.ChunkSize / ((float)_terrainGenerator.GetDetailLevelValue(_lod) - 1f);
        return new Vector3(vector.x * chunkRealSize, vector.y * stepHeight, vector.z * chunkRealSize);
    }
}
