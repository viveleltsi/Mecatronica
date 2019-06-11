using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Terrain generator class
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
    /// <summary>
    /// Prefab for chunk map mesh
    /// </summary>
    public GameObject ChunkPrefab;

    /// <summary>
    /// Container for all chunk
    /// </summary>
    public Transform ChunkContainer;

    /// <summary>
    /// Frequency used (zoom)
    /// </summary>
    [HideInInspector]
    public float Frequency = 42f;

    /// <summary>
    /// Seed for random
    /// </summary>
    [HideInInspector]
    public int Seed = 23544;
    /// <summary>
    /// Number of layers
    /// </summary>
    [HideInInspector]
    public int NbLayer = 4;

    /// <summary>
    /// Details level , 0 to 2
    /// </summary>
    [HideInInspector]
    public int DetailLevel = 0;

    /// <summary>
    /// Detail level value 16 32 64
    /// </summary>
    [HideInInspector]
    public int DetailLevelValue = 16;

    /// <summary>
    /// Get the level of detail needed
    /// </summary>
    /// <param name="lod"></param>
    /// <returns></returns>
    public int GetDetailLevelValue(int lod = -1)
    {
        if (lod == -1)
            return DetailLevelValue;
        else
        {
            return (int)Mathf.Pow(2, lod + 3);
        }
    }

    /// <summary>
    /// Chunk size in Unity Unity
    /// </summary>
    [HideInInspector]
    public float ChunkSize = 100;

    /// <summary>
    /// Size of a step
    /// </summary>
    [HideInInspector]
    public float StepHeight = 1;

    /// <summary>
    /// Setting for the height map generation
    /// </summary>
    [HideInInspector]
    public NoiseSettings Settings;

    /// <summary>
    /// Height curve
    /// </summary>
    public AnimationCurve HeightCurve;

    public float StepValue
    {
        get
        {
            return ChunkSize / DetailLevelValue;
        }
    }

    public float NoiseStep
    {
        get
        {
            return StepValue / Frequency;
        }
    }

    private Stopwatch _chunkGenerationSW;

    /// <summary>
    /// Complete size of a chunk
    /// </summary>
    private float _completeSize => 100f / Frequency;

    /// <summary>
    /// Half size of a chunk
    /// </summary>
    private float _halfSize => _completeSize / 2f;

    /// <summary>
    /// Get the noise increment
    /// </summary>
    /// <param name="levelDetail"></param>
    /// <returns></returns>
    private float GetNoiseIncrement(float levelDetail)
    {
        return _completeSize / (levelDetail - 1f);
    }
    
    /// <summary>
    /// Generate the world
    /// </summary>
    [ExecuteInEditMode]
    [ContextMenu("Generate with octaves")]
    public void GenerateTerrain()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        DeleteAllChunk();
        _chunkGenerationSW = new Stopwatch();
        _chunkGenerationSW.Start();
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                GenerateChunk(new Vector2Int(x,z));
            }
        }
        stopwatch.Start();
        UnityEngine.Debug.Log("Generate chunk in:" + stopwatch.ElapsedMilliseconds + " ms");
    }

    /// <summary>
    /// Get the chunk position by giving the chunk number
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 GetChunkPosition(int x, int y)
    {
        return new Vector3(x, 0f, y) * ChunkSize;
    }

    /// <summary>
    /// Get the world chunk position by giving the chunk position
    /// </summary>
    /// <param name="chunkPosition"></param>
    /// <returns></returns>
    public Vector3 GetChunkPosition(Vector2Int chunkPosition)
    {
        return GetChunkPosition(chunkPosition.x, chunkPosition.y);
    }

    /// <summary>
    /// Get the resource position by giving the chunk position and the local position
    /// </summary>
    /// <param name="chunkPosition"></param>
    /// <param name="resourcePosition"></param>
    /// <returns></returns>
    public Vector3 GetResourcePosition(Vector2Int chunkPosition, Vector2 resourcePosition)
    {
        Debug.Log($"chunk pos ({chunkPosition}) :{ GetChunkPosition(chunkPosition)} -> local pos ({resourcePosition}) :{new Vector3(resourcePosition.x, 0f, resourcePosition.y) * ChunkSize}");
        var position = GetChunkPosition(chunkPosition) + new Vector3(resourcePosition.x, 0f, resourcePosition.y) * ChunkSize;
        return position;
    }

    /// <summary>
    /// Generate chunks
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void GenerateChunk(Vector2Int chunkPosition)
    {
        var realHeight = GenerateLayerMeshAmpl(chunkPosition);
        realHeight = FilterHeightMap(realHeight);
        var meshData = GenerateMeshTerrainStep(realHeight, chunkPosition);
        CreateChunk(meshData, chunkPosition);
    }

    /// <summary>
    /// Generate the chunk data for a specific chunk and level of details
    /// </summary>
    /// <param name="position"></param>
    /// <param name="lod"></param>
    /// <returns></returns>
    public MeshData GenerateChunkData(Vector2Int position, int lod = 0)
    {
        var realHeight = GenerateLayerMeshAmpl(position,lod);
        realHeight = FilterHeightMap(realHeight);
        var meshData = GenerateMeshTerrainStep(realHeight, position,lod);
        return meshData;
    }

    /// <summary>
    /// Filter the height map to correct artefact
    /// </summary>
    /// <param name="heightMap"></param>
    /// <returns></returns>
    public Dictionary<Vector2Int,float> FilterHeightMap(Dictionary<Vector2Int, float> heightMap)
    {
        foreach(var key in heightMap.Keys)
        {
            //TODO filter alone height
        }
        return heightMap;
    }

    /// <summary>
    /// Generate the mesh terrain with step mode
    /// </summary>
    /// <param name="realHeight"></param>
    private MeshData GenerateMeshTerrainStep(Dictionary<Vector2Int, float> realHeight,Vector2Int chunkPosition,int lod = -1)
    {
        var detailLevel = GetDetailLevelValue(lod);
        Vector3[,] grid = new Vector3[detailLevel, detailLevel];
        for (int x = 0; x < detailLevel; x++)
        {
            for (int z = 0; z < detailLevel; z++)
            {
                float y = Mathf.Round(realHeight[new Vector2Int(x, z)] * (float)NbLayer) * (float)StepHeight;
                grid[x, z] = new Vector3((float)x, y, (float)z);
            }
        }
        var meshData = new MeshData(this,lod, chunkPosition);
        if(Application.isPlaying == false && Application.isEditor)
        {
            //Edit mode
            meshData.StartGenerateMeshThreaded(grid);
        }
        else
        {
            var thread = new Thread(new ParameterizedThreadStart(meshData.StartGenerateMeshThreaded));
            thread.Start(grid);
        }
        return meshData;
    }
    
    /// <summary>
    /// Delete all chunk
    /// </summary>
    private void DeleteAllChunk()
    {
        bool inEditMode = Application.isPlaying == false && Application.isEditor;
        for (int i = ChunkContainer.childCount-1; i >= 0 ; i--)
        {
            if (inEditMode)
                DestroyImmediate(ChunkContainer.GetChild(i).gameObject);
            else
                Destroy(ChunkContainer.GetChild(i).gameObject);
        }
    }

    private Dictionary<MeshData, GameObject> _meshDataCurrentlyGenerated = new Dictionary<MeshData, GameObject>();

    /// <summary>
    /// Create a chunk at specific position
    /// </summary>
    /// <param name="meshData"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    private void CreateChunk(MeshData meshData, Vector2Int chunkPosition)
    {
        var newChunk = Instantiate<GameObject>(ChunkPrefab, GetChunkPosition(chunkPosition), Quaternion.identity, ChunkContainer);
        _meshDataCurrentlyGenerated.Add(meshData, newChunk);
        if (Application.isPlaying == false && Application.isEditor)
        {
            //Edit mode
            var mesh = MeshData.GetMesh(meshData);
            newChunk.GetComponent<MeshFilter>().mesh = mesh;
            newChunk.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }

    /// <summary>
    /// Assign mesh from meshdata to the chunk gameobject
    /// </summary>
    /// <param name="meshData"></param>
    private void AssignMesh(MeshData meshData)
    {
        UnityEngine.Debug.Log("Assign mesh for meshdata");
        var newChunk = _meshDataCurrentlyGenerated[meshData];
        var mesh = MeshData.GetMesh(meshData);
        newChunk.GetComponent<MeshFilter>().mesh = mesh;
        var sw = new Stopwatch();
        sw.Start();
        newChunk.GetComponent<MeshCollider>().sharedMesh = mesh;
        Debug.Log($"assign collider mesh {meshData.ChunkPosition} in {sw.ElapsedMilliseconds} ms");
    }

    private void Update()
    {
        if(_meshDataCurrentlyGenerated != null && _meshDataCurrentlyGenerated.Count() > 0)
        {
            for (int i = _meshDataCurrentlyGenerated.Keys.Count()-1; i >= 0; i--)
            {
                var meshData = _meshDataCurrentlyGenerated.Keys.ToList()[i];
                if (meshData.IsTaskDone)
                {
                    AssignMesh(meshData);
                    UnityEngine.Debug.Log($"{i}  meshdata {_meshDataCurrentlyGenerated.Count()}");
                    _meshDataCurrentlyGenerated.Remove(meshData);
                    UnityEngine.Debug.Log($"{i} Removed meshdata {_meshDataCurrentlyGenerated.Count()}");
                }
            }
        }
        else if(_chunkGenerationSW != null)
        {
            _chunkGenerationSW.Stop();
            UnityEngine.Debug.Log($"Finish thread generation {_chunkGenerationSW.ElapsedMilliseconds} ms");
            _chunkGenerationSW = null;
        }
    }

    public float GetLayerHeight(int layerIndex, float nbLayer)
    {
        return (float)layerIndex * StepHeight;
    }

    private float GetLayerHeight(int layerIndex)
    {
        return (float)layerIndex * StepHeight;
    }

    public int GetLayerNumber(float layerHeight,int nbLayer)
    {
        return Mathf.RoundToInt((float)layerHeight / StepHeight);
    }


    /// <summary>
    /// Generate layer data
    /// If the land exist or not for a specific layer min height
    /// </summary>
    /// <param name="minHeight"></param>
    /// <returns></returns>
    private Dictionary<Vector2Int,float> GenerateLayerMesh()
    {
        Dictionary<Vector2Int, float> layerData = new Dictionary<Vector2Int, float>();
        for (int x = 0; x < DetailLevelValue; x++)
        {
            for (int y = 0; y < DetailLevelValue; y++)
            {
                float px = x / Frequency;
                float py = y / Frequency;
                float height = Mathf.PerlinNoise(px,py );
                //Debug.Log($"h:{height} - {px}/{py}");
                Vector2Int position = new Vector2Int(x, y);
                //int layerNumber = Mathf.FloorToInt(height * NbLayer);
                layerData.Add(position, height);
            }
        }
        return layerData;
    }


    /// <summary>
    /// Generate the ressource map
    /// Dont forget to use the seed before
    /// UnityEngine.Random.InitState(Seed);
    /// </summary>
    /// <param name="chunkPosition"></param>
    /// <param name="samplingLevel"></param>
    /// <returns></returns>
    public List<Vector2> GenerateResourceMap(Vector2Int chunkPosition,float minProbabilityHeight, float samplingLevel = 16f)
    {
        List<Vector2> resourcePosition = new List<Vector2>();

        // Increment for noise method
        float completeSize = 100f / Frequency;
        float noiseIncrement = completeSize / (samplingLevel - 1f);
        float halfSize = completeSize / 2f;
        Vector2 offset = new Vector2(
            UnityEngine.Random.Range(0f, 100f) - halfSize,
            UnityEngine.Random.Range(0f, 100f) - halfSize
            );
        Vector2 chunkOffset = new Vector2((float)chunkPosition.x * _completeSize, (float)chunkPosition.y * _completeSize);
        for (int x = 0; x < samplingLevel; x++)
        {
            for (int y = 0; y < samplingLevel; y++)
            {
                var height = UnityEngine.Random.value;
                if(height >= minProbabilityHeight)
                {
                    resourcePosition.Add(new Vector2((x + 1f) / samplingLevel, (y + 1f) / samplingLevel));
                }
            }
        }
        return resourcePosition;
    }


    /// <summary>
    /// Generate Height value
    /// </summary>
    /// <param name="chunkPosition"></param>
    /// <param name="lod"></param>
    /// <returns></returns>
    public Dictionary<Vector2Int,float> GenerateLayerMeshAmpl(Vector2Int chunkPosition, int lod = -1)
    {
        Dictionary<Vector2Int, float> layerData = new Dictionary<Vector2Int, float>();

        // Increment for noise method
        float levelDetail = GetDetailLevelValue(lod);
        float noiseIncrement = GetNoiseIncrement(levelDetail);

        // handle the seed for begin always at the same point
        UnityEngine.Random.InitState(Seed);        
        Vector2 offset = new Vector2(
            UnityEngine.Random.Range(0f, 100f) - _halfSize,
            UnityEngine.Random.Range(0f, 100f) - _halfSize
            );

        Vector2 chunkOffset = new Vector2((float)chunkPosition.x * _completeSize, (float)chunkPosition.y * _completeSize);
        
        for (int x = 0; x < levelDetail; x++)
        {
            for (int y = 0; y < levelDetail; y++)
            {
                float px = x * noiseIncrement + offset.x + chunkOffset.x;
                float py = y * noiseIncrement + offset.y + chunkOffset.y;

                float height = Mathf.PerlinNoise(px, py);
                float range = 1f;
                float frequency = 1f;
                float amplitude = 1f;
                for (int i = 0; i < Settings.NbOctave; i++)
                {
                    frequency *= Settings.Lacunarity;
                    amplitude *= Settings.Persistance;
                    range += amplitude;
                    height += Mathf.PerlinNoise(px / frequency, py / frequency) * amplitude;

                }
                height /= range;
                height = HeightCurve.Evaluate(height);
                Vector2Int position = new Vector2Int(x, y);
                layerData.Add(position, height);
            }
        }
        return layerData;
    }

}