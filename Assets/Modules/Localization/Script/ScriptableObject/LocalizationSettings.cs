using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dan.Localization
{
    /// <summary>
    /// Setting for the localization of the game
    /// </summary>
    [Serializable]
    public class LocalizationSettings : UniqueScriptableObject
    {
        /// <summary>
        /// All language data
        /// </summary>
        [SerializeField]
        public List<LocalizedDataPerLanguage> DataPerLanguages = new List<LocalizedDataPerLanguage>();

        /// <summary>
        /// Default language
        /// </summary>
        [SerializeField]
        public LocalizedLanguage DefaultLanguage;

    }
}
