using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dan.Manager
{
    [CreateAssetMenu(fileName = "Descriptor", menuName = "Dan/Descriptor/Building", order = 1)]
    public class BuildingDescriptor : PrefabDescriptor
    {
        [LinkToScriptableObject(typeof(PlaceholderDescriptor),"PlaceHolder object")]
        public string PlaceholderId;
    }
}
