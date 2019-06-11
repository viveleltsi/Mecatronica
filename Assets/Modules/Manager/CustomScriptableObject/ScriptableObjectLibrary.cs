using Dan.Localization;
using Dan.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "library", menuName = "Manager/Scriptable Object Library", order = 1)]
public class ScriptableObjectLibrary : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<AbstractScriptableObjectElement> _elements = new List<AbstractScriptableObjectElement>();

    [SerializeField]
    private List<string> _elementIds = new List<string>();

    /// <summary>
    /// All descriptors on a dictionary with id as Key
    /// </summary>
    public Dictionary<string, AbstractScriptableObjectElement> Elements = new Dictionary<string, AbstractScriptableObjectElement>();

    /// <summary>
    /// Localization settings
    /// </summary>
    public LocalizationSettings LocalizationSettings;

    public void OnBeforeSerialize()
    {
        if ( Elements.Values.Count > 0)
        {
            _elements = Elements.Values.ToList();
            _elementIds = Elements.Keys.ToList();
        }
        else
        {
            _elements = new List<AbstractScriptableObjectElement>();
            _elementIds = new List<string>();
        }  
    }

    public void OnAfterDeserialize()
    {
        Elements = new Dictionary<string, AbstractScriptableObjectElement>();
        Elements.Clear();
        for (int i = 0; i < _elementIds.Count(); i++)
        {
            Debug.LogWarning("elements:" + _elementIds[i]);
            Elements.Add(_elementIds[i], _elements[i]);
        }
    }
}
