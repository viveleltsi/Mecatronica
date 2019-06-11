using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dan.Manager
{
    [CreateAssetMenu(fileName = "prefabDescription", menuName = "Dan/Descriptor/Prefab", order = 1)]
    public class PrefabDescriptor : Descriptor
    {
        public GameObject Prefab;
    }
}
