using Dan.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropperDescriptor", menuName = "Dan/Descriptor/DropperDescriptor", order = 2)]
public class DropperDescriptor : BuildingDescriptor
{
    public DropperDescriptor()
    {
        Category = "Dropper";
    }
}

