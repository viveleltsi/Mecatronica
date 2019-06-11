using Assets.Scripts.Entity.Building;
using Dan.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle building construction
/// Construction step:
/// Start selecting place -> select place -> construction -> end construction
/// </summary>
public class BuildingManager : Singleton<BuildingManager>
{

    #region Workflow variables
    /// <summary>
    /// Gameobject to show when player choose location for a building
    /// </summary>
    private GameObject _choosingPositionPlaceholder;
    #endregion

    /// <summary>
    /// Name of the gameobject parent to every buildings
    /// </summary>
    private const string BuildingFolderName = "Buildings";


    #region Data
    /// <summary>
    /// Return the data of a building
    /// </summary>
    /// <param name="buildingId">Id of the building</param>
    /// <returns>The building descriptor with the specific buildingId</returns>
    public BuildingDescriptor GetBuildingData(string buildingId)
    {
        var descriptor = ScriptableObjectManager.Instance.GetDescriptor(buildingId);
        if(descriptor != null && descriptor is BuildingDescriptor buildingDescriptor)
        {
            return buildingDescriptor;
        }
        else
        {
            return null;
        }
    }
    #endregion

    #region Construction workflow
    /// <summary>
    /// Start choosing a location for a building
    /// </summary>
    /// <param name="buildingId">Id of the building</param>
    public void StartChoosingLocation(string buildingId)
    {
        //Get the building data
        var data = GetBuildingData(buildingId);
        //Get the right mouse placeholder (prefab)
        var placeholderPrefab = PrefabManager.Instance.GetPrefab(data.PlaceholderId);
        if(placeholderPrefab != null)
        {
            //Set the mouse placeholder (instantiate prefab)
            var groundPosition = InputManager.Cursor.Position.GroundPosition;
            Debug.Log($"Position choosen for building:{groundPosition}");
            _choosingPositionPlaceholder = Instantiate<GameObject>(placeholderPrefab, groundPosition, Quaternion.identity);
            //Set the current building to construct to the placeholder
            _choosingPositionPlaceholder.GetComponent<IPlaceholder>().SetBuildingToConstruct(buildingId);
        }
    }

    /// <summary>
    /// Canceling the choosing location
    /// </summary>
    public void CancelChoosingLocation()
    {
        Destroy(_choosingPositionPlaceholder);
    }

    /// <summary>
    /// Create a building who will NOT be registred or handle by the building manager
    /// For temporary building for example. It can be registred later if needed.
    /// </summary>
    /// <param name="descriptor"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public GameObject CreateBuildingNotRegistred(BuildingDescriptor descriptor, Transform parent)
    {
        return Instantiate<GameObject>(descriptor.Prefab, parent);
    }

    /// <summary>
    /// Create a building
    /// </summary>
    /// <param name="buildingId"></param>
    /// <returns></returns>
    public GameObject CreateBuilding(string buildingId)
    {
        var data = GetBuildingData(buildingId);
        return CreateBuilding(data);
    }

    /// <summary>
    /// Create a building at a specific position and rotation
    /// </summary>
    /// <param name="buildingId"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public GameObject CreateBuilding(string buildingId, Vector3 position, Quaternion rotation)
    {
        var data = GetBuildingData(buildingId);
        return CreateBuilding(data, position, rotation);
    }

    /// <summary>
    /// Create building by using the specific descriptor given as parameter
    /// </summary>
    /// <param name="descriptor"></param>
    /// <returns></returns>
    public GameObject CreateBuilding(BuildingDescriptor descriptor)
    {
        var building = Instantiate<GameObject>(descriptor.Prefab, ParentFolderManager.Instance.GetFolder(BuildingFolderName));
        return building;
    }

    /// <summary>
    /// Create building by giving the description, position and rotation.
    /// </summary>
    /// <param name="descriptor"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public GameObject CreateBuilding(BuildingDescriptor descriptor, Vector3 position, Quaternion rotation)
    {
        var building = Instantiate<GameObject>(descriptor.Prefab, position, rotation, ParentFolderManager.Instance.GetFolder(BuildingFolderName));
        return building;
    }

    #endregion
}

