using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dan.Manager
{
    public abstract class AbstractScriptableObject : ScriptableObject
    {
        /// <summary>
        /// Uniqque Guid of this element
        /// </summary>
        public string Id;

        /// <summary>
        /// Editor name of element
        /// </summary>
        public string EditorName;

        public string Category;

#if UNITY_EDITOR
        private void OnEnable()
        {
            //Assign a random Guid to this element once
            if (string.IsNullOrWhiteSpace(Id))
                Id = Guid.NewGuid().ToString();
        }
#endif
    }
}
