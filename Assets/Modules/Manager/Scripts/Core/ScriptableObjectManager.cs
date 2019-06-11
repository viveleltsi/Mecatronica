using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Reflection;
using Dan.Localization;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Dan.Manager
{
    public class ScriptableObjectManager : Singleton<ScriptableObjectManager>
    {
        /// <summary>
        /// Library files
        /// </summary>
        public ScriptableObjectLibrary Library;

        /// <summary>
        /// Localization settings
        /// </summary>
        public LocalizationSettings LocalizationSetting => Library.LocalizationSettings;

        /// <summary>
        /// Get the prefabDescriptor
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Descriptor GetDescriptor(string id)
        {
            if (Library.Elements.ContainsKey(id) && Library.Elements[id] is Descriptor)
                return Library.Elements[id] as Descriptor;
            else
                return null;
        }

        /// <summary>
        /// Get abstract Scriptable object element from library
        /// </summary>
        /// <param name="elementId"></param>
        /// <returns></returns>
        public AbstractScriptableObjectElement GetAbstractScriptableObjectElement(string elementId)
        {
            if (Library.Elements.ContainsKey(elementId))
            {
                return Library.Elements[elementId];
            }
            else
            {
                return null;
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Update the library
        /// </summary>
        public static void UpdateLibrary()
        {
            var instanceAndPath = GetAllInstancesAndPaths<ScriptableObjectLibrary>();
            if (instanceAndPath.objects.Length == 0)
                Debug.LogError("No Library found");
            else if (instanceAndPath.objects.Length > 1)
                Debug.LogError($"More than one library found ({instanceAndPath.objects.Length})");
            else
            {
                var library = instanceAndPath.objects.FirstOrDefault();

                Debug.Log($"<color=orange>{library.Elements.Keys.Count()}</color> descriptor already saved.");
                var elements = GetAllInstances<AbstractScriptableObjectElement>().ToList();
                Debug.Log($"Elements found: {elements.Count()}");
                foreach (var d in elements)
                {
                    Debug.Log("found:" + d.GetType().Name +" : "+d.name);
                }

                library.Elements.Clear();
                foreach (var d in elements)
                {
                    library.Elements.Add(d.Id, d);
                }

                Debug.Log($"<color=green>{library.Elements.Keys.Count()}</color> element on library.");

                
                //Localization
                var localizationSetting = GetAllInstances<LocalizationSettings>().FirstOrDefault();
                library.LocalizationSettings = localizationSetting;
                EditorUtility.SetDirty(library);
            }
        }

        /// <summary>
        /// Get all instance of a scriptable object type
        /// Editor function only
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return a;
        }

        /// <summary>
        /// Get all instance of scriptable objects and theirs path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (string[] paths, T[] objects) GetAllInstancesAndPaths<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] assets = new T[guids.Length];
            string[] paths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
            {
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(paths[i]);
            }
            return (paths,assets);
        }

        public static IEnumerable<T> GetEnumerableOfType<T>() where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type));
            }
            return objects;
        }
#endif
    }
}
