using Assets.Scripts.Entity.Building;
using Dan.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropPlaceholderBehaviour : MonoBehaviour, IPlaceholder
{
    /// <summary>
    /// Material for placeholder item
    /// </summary>
    public Material HoloMaterial;

    /// <summary>
    /// Link to the dropper to use
    /// </summary>
    [LinkToScriptableObject(typeof(DropperDescriptor), "Dropper link")]
    public string DropperId;

    /// <summary>
    /// Descriptor of the building to create
    /// </summary>
    private BuildingDescriptor _buildingDescriptor;

    /// <summary>
    /// Copy of the placeholder object with holo material
    /// </summary>
    private GameObject _placeholderObject;

    private GameObject _dropper = null;

    public void SetBuildingToConstruct(string buildingId)
    {
        Debug.Log("Set the building to construct to id:" + buildingId);
        _buildingDescriptor = BuildingManager.Instance.GetBuildingData(buildingId);
        _placeholderObject = Instantiate<GameObject>(_buildingDescriptor.Prefab,transform,false);
        SetMaterial(_placeholderObject.GetComponentsInChildren<MeshRenderer>(),HoloMaterial);
        SetMaterial(_placeholderObject.GetComponents<MeshRenderer>(), HoloMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        if(_dropper == null)
        {
            if (Input.GetMouseButtonUp(0) && _placeholderObject != null)
            {
                Destroy(_placeholderObject);
                _placeholderObject = null;
                //Call the dropper
                CallDropper();
            }
            else
            {
                transform.position = InputManager.Cursor.Position.GroundPosition;
            }
        }
    }

    private void CallDropper()
    {
        var descriptor = BuildingManager.Instance.GetBuildingData(DropperId);
        _dropper = Instantiate<GameObject>(descriptor.Prefab, transform.position + Vector3.up * 1200f, Quaternion.identity);
        var dropperScript = _dropper.GetComponent<IDropperBehaviour>();
        dropperScript.StartFalling(transform.position, _buildingDescriptor, DropFinished);
    }

    private void DropFinished()
    {
        _dropper.GetComponent<IDropperBehaviour>().FlyAway();
    }

    private void ConstructBuilding()
    {
        var building = BuildingManager.Instance.CreateBuilding(_buildingDescriptor, transform.position, Quaternion.identity);
    }

    private void EndOfPlaceholder()
    {
        Destroy(this.gameObject);
        Destroy(this);
    }

    private void SetMaterial(IEnumerable<MeshRenderer> renderers, Material material)
    {
        foreach(var renderer in renderers)
        {
            var mats = renderer.materials;
            //mats.ToList().ForEach(x => x = material);
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = material;
            }
            renderer.materials = mats;
        }
    }
}
