    using System;
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator generator = (TerrainGenerator)target;
        EditorGUI.BeginChangeCheck();
        generator.Seed = EditorGUILayout.IntField("Seed", generator.Seed);
        if(GUILayout.Button("Random seed"))
        {
            generator.Seed = UnityEngine.Random.Range(0, 50000);
        }

        GUILayout.Space(10f);
        GUILayout.Label("Noise setting");
        generator.Frequency = EditorGUILayout.Slider("Zoom", generator.Frequency, 20f, 500f);
        generator.Settings.NbOctave = EditorGUILayout.IntSlider("Octaves", generator.Settings.NbOctave, 0, 15);
        generator.Settings.Persistance = EditorGUILayout.Slider ("Persistance", generator.Settings.Persistance, 0.1f, 1f);
        generator.Settings.Lacunarity = EditorGUILayout.Slider("Lacunarity", generator.Settings.Lacunarity, 0.1f, 1f);

        GUILayout.Space(10f);
        GUILayout.Label("Height setting");
        generator.NbLayer = EditorGUILayout.IntSlider("Number of layer",generator.NbLayer, 1, 30);
        generator.StepHeight = EditorGUILayout.FloatField("Height of a step [U]", generator.StepHeight);

        GUILayout.Space(10f);
        GUILayout.Label("Size details");
        generator.ChunkSize = EditorGUILayout.FloatField("Size of a chunk [U]", generator.ChunkSize);
        generator.DetailLevel = EditorGUILayout.IntSlider("Detail level", generator.DetailLevel, 0, 2);
        generator.DetailLevelValue = (int)Mathf.Pow(2, generator.DetailLevel + 3);


        if (GUI.changed)
        {
            generator.GenerateTerrain();
        }
        EditorGUI.EndChangeCheck();

        if (GUILayout.Button("Generate"))
            generator.GenerateTerrain();

        base.DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
        
    }
}
