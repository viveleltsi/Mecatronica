using Dan.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dan.Localization
{

    public class LocalizationManager : Singleton<LocalizationManager>
    {

        public LocalizationSettings LocalizationSettings;

        /// <summary>
        /// All localization text data per language
        /// </summary>
        private List<LocalizedDataPerLanguage> _datas = new List<LocalizedDataPerLanguage>();

        /// <summary>
        /// All data sorted by language then by id
        /// </summary>
        private Dictionary<LocalizedLanguage, LocalizedDataPerLanguage> _dataPerLanguage = new Dictionary<LocalizedLanguage, LocalizedDataPerLanguage>();

        /// <summary>
        /// All data order by id then by language
        /// </summary>
        private Dictionary<string, LocalizedKey> _allKeys = new Dictionary<string, LocalizedKey>();

        /// <summary>
        /// All languages availables
        /// </summary>
        private List<LocalizedLanguage> _languages = new List<LocalizedLanguage>();

        /// <summary>
        /// Constructor
        /// </summary>
        public LocalizationManager()
        {
            LoadAllTexts();
        }

        /// <summary>
        /// Get the text in raw format
        /// </summary>
        /// <param name="id"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public string GetRawText(string id, LocalizedLanguage language)
        {
            string text = "";
            if (_allKeys.ContainsKey(id))
                text = _allKeys[id].GetText(language);
            return text;
        }

        /// <summary>
        /// Get the text formatted with its tag
        /// </summary>
        /// <param name="id"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public string GetFormattedText(string id, LocalizedLanguage language)
        {
            var rawText = GetRawText(id, language);
            return ManageSpecialTag(rawText);
        }

        /// <summary>
        /// Handle special tag
        /// </summary>
        private string ManageSpecialTag(string text)
        {
            //TODO Manage tag
            return text;
        }


        /// <summary>
        /// Load all quests
        /// </summary>
        private void LoadAllTexts()
        {
            _datas = LocalizationSettings.DataPerLanguages;
            List<string> ids = new List<string>();
            foreach (var dataPerLanguage in _datas)
            {
                _dataPerLanguage.Add(dataPerLanguage.Language, dataPerLanguage);
                _languages.Add(dataPerLanguage.Language);
                foreach(var text in dataPerLanguage.Texts)
                {
                    ids.Add(text.Id);
                }
            }
            foreach(var id in ids)
            {
                List<LocalizedText> texts = new List<LocalizedText>();
                List<LocalizedLanguage> languages = new List<LocalizedLanguage>();
                foreach(var data in _dataPerLanguage)
                {
                    languages.Add(data.Key);
                    texts.Add(data.Value.Texts.FirstOrDefault(x => x.Id == id));
                }
                LocalizedKey key = new LocalizedKey(id, texts, languages);
                _allKeys.Add(id, key);
            }
        }
    }
}
