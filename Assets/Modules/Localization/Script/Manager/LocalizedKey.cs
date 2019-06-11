using System.Collections.Generic;
using System.Linq;

namespace Dan.Localization
{
    public class LocalizedKey
    {
        /// <summary>
        /// Id of the key
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// All text order by languages
        /// </summary>
        private Dictionary<LocalizedLanguage, string> _texts = new Dictionary<LocalizedLanguage, string>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="texts"></param>
        /// <param name="languages"></param>
        public LocalizedKey(string id, List<LocalizedText> texts, List<LocalizedLanguage> languages)
        {
            Id = id;
            _texts = new Dictionary<LocalizedLanguage, string>();
            for (int i = 0; i < texts.Count(); i++)
            {
                _texts.Add(languages[i], texts[i].Text);
            }
        }

        /// <summary>
        /// Get the text for a specific language
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public string GetText(LocalizedLanguage language)
        {
            string value = "";
            if (_texts.ContainsKey(language))
            {
                value = _texts[language];
            }
            return value;
        }
    }
}
