using Dan.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "SmoothLoaderProfile", menuName = "Dan/SmoothLoader")]
public class SmoothLoaderOptions : ScriptableObject
{
    [SerializeField]
    public List<string> DisablingScriptExceptions = new List<string>();

    private List<Type> DisablingClassExceptions = new List<Type>()
    {
        typeof(PostProcessVolume),
        typeof(PostProcessLayer),
        typeof(Singleton),
    };

    public bool IsValidForSmoothLoader(MonoBehaviour script)
    {
        if (DisablingScriptExceptions.Contains(script.GetType().ToString()) == false
            && DisablingClassExceptions.Contains(script.GetType()) == false
#if UNITY_EDITOR
#endif
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
