using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dan.Localization
{
    [Serializable]
    public class LocalizedLanguage : UniqueObject
    {
        public bool IsInitialized()
        {
            return string.IsNullOrWhiteSpace(Id) == false && string.IsNullOrWhiteSpace(EditorName) == false;
        }
    }
}
