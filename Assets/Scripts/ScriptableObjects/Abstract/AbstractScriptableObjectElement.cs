using Dan.Manager;
using System;
using UnityEngine;

public abstract class AbstractScriptableObjectElement: AbstractScriptableObject
{
    /// <summary>
    /// Id of the localized name
    /// </summary>
    public string LocalizedNameId;

    /// <summary>
    /// Id of the localized description
    /// </summary>
    public string LocalizedDescriptionId;

    /// <summary>
    /// Icon of the element
    /// </summary>
    public Sprite Icon;

    #region Variable Getter

    /// <summary>
    /// Get the localized name of this element
    /// </summary>
    public string Name
    {
        get
        {
            if (string.IsNullOrWhiteSpace(LocalizedNameId))
                return "";
            else
            {
                return LocalizedDescriptionId;  //TODO Localization system
            }
        }
    }

    /// <summary>
    /// Get the localized description of this element
    /// </summary>
    public string Description
    {
        get
        {
            if (string.IsNullOrWhiteSpace(LocalizedNameId))
                return "";
            else
            {
                return LocalizedDescriptionId;  //TODO Localization system
            }
        }
    }

    #endregion


}