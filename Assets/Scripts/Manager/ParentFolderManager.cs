using Dan.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dan.Manager
{
    public class ParentFolderManager : Singleton<ParentFolderManager>
    {
        /// <summary>
        /// List of all folders
        /// </summary>
        public Dictionary<string, Transform> _folders = new Dictionary<string, Transform>();
        
        /// <summary>
        /// Get the specific folder
        /// Create it if needed
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Transform GetFolder(string name)
        {
            if (_folders.ContainsKey(name))
            {
                if(_folders[name] != null){
                    return _folders[name];
                }
                else
                {
                    _folders.Remove(name);
                    return CreateFolder(name);
                }
            }
            else
            {
                return CreateFolder(name);
            }
        }

        /// <summary>
        /// Create a folder and store it on the dictionary
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Transform CreateFolder(string name)
        {
            var go = new GameObject();
            go.name = name;
            _folders.Add(name, go.transform);
            return go.transform;
        }
    }
}
