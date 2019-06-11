
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Settings for the generating noise
/// </summary>
[Serializable]
public class NoiseSettings
{
    public int NbOctave;
    [Range(1f,4f)]
    public float Persistance;
    [Range(0f, 1f)]
    public float Lacunarity;
}

/// <summary>
/// Old way to hangle the generation noise
/// </summary>
[Obsolete("Use NoiseSetting insteed")]
[Serializable]
public class Frequency
{
    
    [Range(0f,1f)]
    public float Amplitude;

    /// <summary>
    /// must be a multiple of two
    /// </summary>
    [Range(2,256)]
    public int Harmonic;
}