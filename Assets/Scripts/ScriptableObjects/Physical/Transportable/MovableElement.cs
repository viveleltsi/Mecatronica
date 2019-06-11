using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Movable", menuName = "Mecatronica/Element/Movable")]
public class MovableElement : AbstractTransportableElement
{
    [Header("Movement speed [m/s]")]
    public float Speed;
}
