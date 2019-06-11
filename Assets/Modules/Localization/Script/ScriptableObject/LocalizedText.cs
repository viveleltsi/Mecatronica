using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dan.Localization
{
    /// <summary>
    /// Localized file for a specific language and text
    /// </summary>
    [Serializable]
    public class LocalizedText : UniqueScriptableObject
    {
        /// <summary>
        /// Hint or description of the localization text
        /// </summary>
        public string Hint { get; set; }

        /// <summary>
        /// Category of the text, it's used to make tab on export file
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// text who is localized
        /// </summary>
        public string Text { get; set; }
    }
}

