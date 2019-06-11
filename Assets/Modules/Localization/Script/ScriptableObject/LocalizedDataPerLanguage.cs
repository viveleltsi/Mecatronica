using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dan.Localization
{
    /// <summary>
    /// Data (all key) for a specific language
    /// </summary>
    [Serializable]
    public class LocalizedDataPerLanguage : UniqueObject
    {
        /// <summary>
        /// All localization text
        /// </summary>
        [SerializeField]
        public List<LocalizedText> Texts { get; set; } = new List<LocalizedText>();

        [SerializeField]
        public LocalizedLanguage Language { get; set; }
    }
}
