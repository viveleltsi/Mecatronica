using Dan.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTestBehaviour : MonoBehaviour
{

    public BuildingDescriptor DropDescriptor;

    [LinkToScriptableObject(typeof(BuildingDescriptor), "Rover Descriptor")]
    public string RoverDescriptor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Drop!");
            BuildingManager.Instance.StartChoosingLocation(DropDescriptor.Id);
        } else if(Input.GetKeyDown(KeyCode.R))
        {
            BuildingManager.Instance.StartChoosingLocation(RoverDescriptor);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            BuildingManager.Instance.CancelChoosingLocation();
        }
    }
}
