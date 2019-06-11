using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dan.Manager
{
    /// <summary>
    /// Handle the prefabs
    /// </summary>
    public class PrefabManager : Singleton<PrefabManager>
    {
        /// <summary>
        /// Get a specific prefab
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefabId"></param>
        /// <returns></returns>
        public GameObject GetPrefab(string prefabId)
        {
            if (ScriptableObjectManager.Instance.GetDescriptor(prefabId) is PrefabDescriptor descriptor)
            {
                return descriptor.Prefab;
            }
            return null;
        }
    }
}
