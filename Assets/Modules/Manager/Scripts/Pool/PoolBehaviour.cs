using Dan.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dan.Manager
{

    public class PoolBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Link to the prefab to pool
        /// </summary>
        [LinkToScriptableObject(typeof(PrefabDescriptor), "Prefab link")]
        public string PrefabId;

        /// <summary>
        /// How many object need to be initialized at start (or before start on editor)
        /// </summary>
        public int BaseQty = 0;

        /// <summary>
        /// All unused objects of the pools
        /// </summary>
        [SerializeField]
        public List<GameObject> UnusedObjects = new List<GameObject>();

        /// <summary>
        /// All used objects of the pool
        /// </summary>
        [SerializeField]
        public List<GameObject> UsedObjects = new List<GameObject>();

        [ContextMenu("Get Info")]
        public void GetInfo()
        {
            Debug.Log($"Unused {UnusedObjects.Count()}");
        }

        /// <summary>
        /// Get a object from the pool
        /// </summary>
        /// <returns></returns>
        public GameObject GetObject()
        {
            GameObject newObject = null;
            if (UnusedObjects.Count > 0)
            {
                newObject = UnusedObjects.Last();
                UnusedObjects.Remove(newObject);
            }
            else
            {
                newObject = CreateObject();
            }
            UsedObjects.Add(newObject);
            return newObject;
        }

        /// <summary>
        /// Create a new object
        /// </summary>
        /// <param name="hidden"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public GameObject CreateObject(bool hidden = false, GameObject model = null)
        {
            Vector3 position = Vector3.zero;
            if (hidden)
            {
                position = Vector3.one * -5000f;
            }
            if (model == null)
            {
                model = PrefabManager.Instance.GetPrefab(PrefabId);
            }
            var newObject = Instantiate<GameObject>(
                model,
                position,
                Quaternion.identity,
                transform);
            return newObject;
        }

        public void ReleaseObject(GameObject gameobject)
        {
            if (UsedObjects.Contains(gameobject))
            {
                UsedObjects.Remove(gameobject);
                gameobject.transform.position = Vector3.one * -5000f;
                UnusedObjects.Add(gameobject);
            }
        }


#if UNITY_EDITOR
        /// <summary>
        /// Initialize a pool
        /// </summary>
        [ExecuteInEditMode]
        [ContextMenu("Initialize pool")]
        public void InitPool()
        {
            if (Application.isEditor && Application.isPlaying == false)
            {
                DeleteAllObjets();
                UnusedObjects = new List<GameObject>();
                UsedObjects = new List<GameObject>();
            }
            var allPrefab = ScriptableObjectManager.GetAllInstances<PrefabDescriptor>();
            var newObjectModel = allPrefab.FirstOrDefault(x => x.Id == PrefabId);
            for (int i = 0; i < BaseQty; i++)
            {
                var newObject = CreateObject(true, newObjectModel.Prefab);
                UnusedObjects.Add(newObject);
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// Delete all objects
        /// </summary>
        public void DeleteAllObjets()
        {
            if (transform.childCount > 0)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
        }

#endif
    }
}
