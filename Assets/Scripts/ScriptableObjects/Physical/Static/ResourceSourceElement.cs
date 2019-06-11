using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceSource", menuName = "Mecatronica/Element/Resource source")]
public class ResourceSourceElement : AbstractStaticElement
{
    [LinkToScriptableObject(typeof(RawResourceElement), "Raw resource available")]
    public string RawResourceId;
}
