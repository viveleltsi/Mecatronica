using Dan.Manager;
using Dan.UIWindow;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WikiUIBehaviour : MonoBehaviour
{

    public WindowUIBehaviour UIWindowBehaviour;

    /// <summary>
    /// Wiki icon
    /// </summary>
    [SerializeField]
    private Image _icon;

    /// <summary>
    /// Title
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _title;

    /// <summary>
    /// Description of the entity
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _description;
    
    /// <summary>
    /// Attribute the information of the element to the UI window
    /// </summary>
    /// <param name="details"></param>
    public void SetWikiInformations(AbstractScriptableObjectElement details)
    {
        _icon.sprite = details.Icon;
        _title.text = details.Name;
        _description.text = details.Description;
    }

    /// <summary>
    /// Set the wiki information by givin the element id
    /// </summary>
    /// <param name="elementId"></param>
    public void SetWikiInformations(string elementId)
    {
        var information = ScriptableObjectManager.Instance.GetAbstractScriptableObjectElement(elementId);
        if (information != null)
            SetWikiInformations(information);
    }
}
