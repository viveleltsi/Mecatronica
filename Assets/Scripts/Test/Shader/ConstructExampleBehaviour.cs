using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructExampleBehaviour : MonoBehaviour
{
    public MeshRenderer Renderer;

    public float MaxHeight = 1f;
    public float MinHeight = 0f;
    public float ActualHeight = 0f;
    public float ConstructSpeed = 1f;


    public void Update()
    {
        if (ActualHeight >= MaxHeight)
            ActualHeight = MinHeight;
        ActualHeight += Time.deltaTime * ConstructSpeed;
        foreach(var material in Renderer.materials)
        {
            material.SetFloat("_ConstructHeight", ActualHeight);
        }
    }
}
