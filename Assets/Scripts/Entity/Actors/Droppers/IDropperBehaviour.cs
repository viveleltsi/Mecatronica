using Dan.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IDropperBehaviour
{
    void StartFalling(Vector3 positionToLand, BuildingDescriptor buildingDescriptor, Action DropFinished);

    void FlyAway();
}
