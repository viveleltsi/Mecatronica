using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dan.Localization
{
    /// <summary>
    /// Scriptable object with an unique ID (Guid in string format) and a Editor Name
    /// </summary>
    [Serializable]
    public class UniqueScriptableObject : ScriptableObject
    {

        /// <summary>
        /// Unique Guid
        /// </summary>
        public string Id;

        /// <summary>
        /// Name of the custom scriptable object used for editor interface
        /// </summary>
        public string EditorName;


        public virtual string GetDropDownName()
        {
            return EditorName;
        }
        
        /// <summary>
        /// Generate a guid (replace the actual id)
        /// </summary>
        public void GenerateGuid()
        {
            Id = System.Guid.NewGuid().ToString();
        }
    }
}
