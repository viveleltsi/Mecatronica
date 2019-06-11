using Dan.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlaceholderDescriptor", menuName = "Dan/Descriptor/Placeholder", order = 1)]
public class PlaceholderDescriptor : PrefabDescriptor
{
    public PlaceholderDescriptor()
    {
        Category = "Placeholder";
    }
}
