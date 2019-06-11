using Dan.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Endless terrain generator
/// Developement steps :
/// 1. Make it work     [✓]
/// 2. Make it right    [⌛]
/// 3. Make it fast     [  ]
/// TODO : Refactor when resources generations will be done.
/// </summary>
public class EndlessTerrainGenerator : MonoBehaviour
{
    /// <summary>
    /// Maximum number of chunk to generate on one frame
    /// </summary>
    public int MaxChunkGeneratePerFrame = 20;

    /// <summary>
    /// Max chunk distance to be generated
    /// </summary>
    public float MaxViewDistance = 1000f;

    /// <summary>
    /// Link to the generator
    /// </summary>
    public TerrainGenerator Generator;

    /// <summary>
    /// Chunk pool
    /// </summary>
    public PoolBehaviour TerrainChunkPool;

    /// <summary>
    /// Pool of resource gameobject
    /// TODO make a resourcePoolBehaviour who handle all resource source
    /// </summary>
    public PoolBehaviour ResourceSourcePool;

    /// <summary>
    /// Link to the camera
    /// </summary>
    public Camera Camera;

    /// <summary>
    /// View distance
    /// </summary>
    public float[] ViewDistances = new float[2];

    /// <summary>
    /// Data of all chunks
    /// </summary>
    private Dictionary<Vector2Int, ChunkData> _chunkDatas = new Dictionary<Vector2Int, ChunkData>();

    /// <summary>
    /// Camera view point (in world position)
    /// </summary>
    private CameraViewPoints _cameraViewPoints = new CameraViewPoints();

    /// <summary>
    /// The chunk size
    /// </summary>
    private float ChunkSize
    {
        get
        {
            return Generator.ChunkSize;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _cameraViewPoints.CheckBoundary(Camera,MaxViewDistance);
        List<Vector2Int> existingChunk = new List<Vector2Int>();
        //Check
        for (int i = ViewDistances.Length-1; i >= 0; i--)
        {
            existingChunk.AddRange(CheckMeshAndUpdate(_cameraViewPoints, i));
        }
        var chunkTodelete = _chunkDatas.Where(x => x.Value.GameObject != null && existingChunk.Contains(x.Key) == false).Select(x=> x.Key).ToList();
        RemoveChunk(chunkTodelete);
        CheckMeshCollider(InputManager.Cursor.Position.GroundPosition);
    }

    private void CheckMeshCollider(Vector3 positionOnGround)
    {
        var chunkPosition = GetChunkNumber(positionOnGround);
        var chunk = _chunkDatas.ContainsKey(chunkPosition)? _chunkDatas[chunkPosition] : null;
        if(chunk != null)
        {
            Mesh mesh = null;
            if (chunk.Meshs.ContainsKey(chunk.CurrentLod) && chunk.Meshs[chunk.CurrentLod] != null)
                mesh = chunk.Meshs[chunk.CurrentLod];
            else
                mesh = chunk.Meshs.Where(x => x.Value != null).FirstOrDefault().Value;
            if (mesh != null && chunk != null && chunk.GameObject != null)
            {
                var collider = chunk.GameObject.GetComponent<MeshCollider>();
                if(collider != null)
                    collider.sharedMesh = mesh;
            }
        }
    }

    /// <summary>
    /// Remove chunks at specific position
    /// </summary>
    /// <param name="chunkToDelete"></param>
    private void RemoveChunk(List<Vector2Int> chunkToDelete)
    {
        foreach(var coordinate in chunkToDelete)
        {
            //Debug.Log($"release chunk:{coordinate}");
            TerrainChunkPool.ReleaseObject(_chunkDatas[coordinate].GameObject);
            _chunkDatas[coordinate].GameObject = null;

            _chunkDatas[coordinate].MeteoricIronGameobject.ForEach(x => ResourceSourcePool.ReleaseObject(x));
            _chunkDatas[coordinate].MeteoricIronGameobject.Clear();
        }
    }

    /// <summary>
    /// Check all mesh and return what mesh is already generated
    /// </summary>
    /// <param name="cameraPoint"></param>
    /// <param name="lod"></param>
    /// <returns></returns>
    private List<Vector2Int> CheckMeshAndUpdate(CameraViewPoints cameraPoint,int lod)
    {
        List<Vector2Int> existingChunks = new List<Vector2Int>();
        var viewDistance = ViewDistances[lod];
        Vector3 viewMin = cameraPoint.MinPoint - viewDistance * Vector3.one;
        Vector3 viewMax = cameraPoint.MaxPoint + viewDistance * Vector3.one;

        Vector2Int minChunk = GetChunkNumber(viewMin);
        Vector2Int maxChunk = GetChunkNumber(viewMax);
        var totalVector = maxChunk - minChunk;
        int total = totalVector.x + totalVector.y;
        if(total >= MaxChunkGeneratePerFrame)
        {
            Debug.Log($"To much chunk to generate : {total} / {MaxChunkGeneratePerFrame}");
            return existingChunks;
        }
        for (int x = minChunk.x; x <= maxChunk.x; x++)
        {
            for (int y = minChunk.y; y < maxChunk.y; y++)
            {
                CreateIfNotExisting(x, y,lod);
                existingChunks.Add(new Vector2Int(x, y));
            }
        }
        return existingChunks;
    }

    /// <summary>
    /// Create a chunk if not existing
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void CreateIfNotExisting(int x, int y, int lod = 0)
    {
        Vector2Int position = new Vector2Int(x, y);
        if (IsChunkExist(position) == false)
        {
            CreateChunkData(position,lod);
            CreatingMeshData(position, lod);
        }
        if (IsVisible(position,lod)==false){
            if (HasMeshGenerated(position,lod))
            {
                GenerateGameObjectAndAssignMesh(position, lod);
            }
            else
            {
                if (IsMeshDataExist(position, lod) )
                {
                    if (HasMeshFinishGenerated(position, lod))
                    {
                        CreateMeshFromData(position, lod);
                        GenerateGameObjectAndAssignMesh(position, lod);
                    }
                    else
                    {
                        //We have to wait until it's finished
                    }
                }
                else
                {
                    CreatingMeshData(position,lod);
                }
            }
        }
        else
        {
            //Model exist and visible nothing to do here
        }
    }

    /// <summary>
    /// Creating the mesh data for specific chunk position and specific level of detail
    /// </summary>
    /// <param name="position"></param>
    /// <param name="lod"></param>
    private void CreatingMeshData(Vector2Int position, int lod)
    {
        var newMeshData = Generator.GenerateChunkData(position, lod);
        _chunkDatas[position].Datas.Add(lod, newMeshData);
    }

    /// <summary>
    /// Generate the gameobject and assign the mesh to the mesh renderer
    /// </summary>
    /// <param name="position"></param>
    /// <param name="lod"></param>
    private void GenerateGameObjectAndAssignMesh(Vector2Int position, int lod)
    {
        var valuePair = _chunkDatas[position].Meshs.OrderBy(x => x.Key).Last(x => x.Value != null);
        var newChunk = _chunkDatas[position].GameObject == null ? TerrainChunkPool.GetObject() :
            _chunkDatas[position].GameObject;
        _chunkDatas[position].GameObject = newChunk;
        _chunkDatas[position].CurrentLod = valuePair.Key;
        
        var mesh = valuePair.Value;
        newChunk.GetComponent<MeshFilter>().mesh = mesh;

        //Create resources
        if(lod == 2)
        {
            foreach (var resource in _chunkDatas[position].MeteoricIronPositions)
            {
                var newResource = ResourceSourcePool.GetObject();
                _chunkDatas[position].MeteoricIronGameobject.Add(newResource);
                newResource.transform.position = Generator.GetResourcePosition(position, resource);
            }
        }
        newChunk.transform.position = Generator.GetChunkPosition(position.x, position.y);
    }

    /// <summary>
    /// Create the mesh by using already generated data
    /// </summary>
    /// <param name="position"></param>
    /// <param name="lod"></param>
    private void CreateMeshFromData(Vector2Int position, int lod)
    {
        var valuePair = _chunkDatas[position].Datas.OrderBy(x => x.Key).Last(x => x.Value.IsTaskDone);
        var newMesh = MeshData.GetMesh(valuePair.Value);
        _chunkDatas[position].Meshs.Add(valuePair.Key, newMesh);
    }

    /// <summary>
    /// Create the chunk data
    /// </summary>
    /// <param name="position"></param>
    /// <param name="lod"></param>
    private void CreateChunkData(Vector2Int position,int lod)
    {
        var newData = new ChunkData();
        _chunkDatas.Add(position, newData);
        
        //TODO handle the randomness of the resources
        UnityEngine.Random.InitState(12);   // For now we hardcode the random for the resource (for debug purpose)
        newData.MeteoricIronPositions = Generator.GenerateResourceMap(position, 0.8f,4f);
    }

    private bool IsMeshDataExist(Vector2Int position, int lod)
    {
        if (_chunkDatas.ContainsKey(position) && _chunkDatas[position].Datas.Count(x => x.Key >= lod) > 0)
            return true;
        else
            return false;
    }

    private bool HasMeshFinishGenerated(Vector2Int position,int lod)
    {
        if (_chunkDatas[position].Datas.Count(x => x.Key >= lod && x.Value.IsTaskDone) > 0)
            return true;
        else
            return false;
    }

    private bool IsChunkExist(Vector2Int position)
    {
        return _chunkDatas.ContainsKey(position);
    }

    private bool HasMeshGenerated(Vector2Int position, int lod)
    {
        if(_chunkDatas[position].Meshs.Keys.Count(x =>x >= lod) > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsVisible(Vector2Int position, int lod)
    {
        if (_chunkDatas.ContainsKey(position) && _chunkDatas[position].GameObject != null && _chunkDatas[position].CurrentLod >= lod)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Transform a world position to chunk position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Vector2Int GetChunkNumber(Vector3 position)
    {
        var x = Mathf.FloorToInt(position.x / ChunkSize);
        var y = Mathf.FloorToInt(position.z / ChunkSize);
        return new Vector2Int(x, y);
    }
}
