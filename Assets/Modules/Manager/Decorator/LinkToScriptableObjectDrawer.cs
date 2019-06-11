#if UNITY_EDITOR
using Dan.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


[CustomPropertyDrawer(typeof(LinkToScriptableObjectAttribute))]
public class LinkToScriptableObjectDrawer : PropertyDrawer
{

    public LinkToScriptableObjectAttribute LinkToScriptable
    {
        get { return ((LinkToScriptableObjectAttribute)attribute); }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get all ids
        var type = LinkToScriptable.Type;
        var method = typeof(ScriptableObjectManager).GetMethod("GetAllInstances");
        var genericMethod = method.MakeGenericMethod(type);
        var descriptors = (IEnumerable<AbstractScriptableObject>)genericMethod.Invoke(null, null);

        // Create ids list and popup text
        List<string> ids = descriptors.Select(x => x.Id).ToList();
        List<string> dropdownTexts = new List<string>();
        dropdownTexts.Add("*Select a building on the list*");
        foreach(var descriptor in descriptors)
        {
            string name = $"{descriptor.name} ({descriptor.Id.Substring(0, Mathf.Min(5, descriptor.Id.Length))}..)";
            if(string.IsNullOrWhiteSpace(descriptor.Category) == false)
            {
                name = $"{descriptor.Category}/{name}";
            }
            else
            {
                var splits = descriptor.GetType().ToString().Split('.');
                if (splits.Length > 0)
                {
                    var typeName = splits[splits.Length - 1];
                    if (string.IsNullOrWhiteSpace(typeName) == false)
                    {
                        name = $"{typeName}/{name}";
                    }
                }
            }
            dropdownTexts.Add(name);
        }
        //dropdownTexts.AddRange(descriptors.Select(x => x.name + $" ({x.Id.Substring(0, Mathf.Min(5, x.Id.Length))}..)").ToArray());

        // Look for the initial position
        int initialPosition = 0;
        string actualId = property.stringValue;
        string newId = actualId;
        if (string.IsNullOrWhiteSpace(actualId) == false && ids.Contains(actualId))
        {
            initialPosition = ids.IndexOf(actualId)+1;  // +1 because of the first line for the null value
        }

        // Show popup
        string newLabel = string.IsNullOrWhiteSpace(LinkToScriptable.CustomLabel) ? label.text : LinkToScriptable.CustomLabel;
        int newPosition = EditorGUI.Popup(position, newLabel, initialPosition, dropdownTexts.ToArray());

        // Popup change
        if (newPosition != initialPosition)
        {
            if (newPosition == 0)
                newId = null;
            else
                newId = ids[newPosition - 1];   // Because of the first fake line on the dropdownTexts
        }

        // Save change if needed
        if(newId != actualId)
        {
            property.stringValue = newId;
            property.serializedObject.ApplyModifiedProperties();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}
#endif
