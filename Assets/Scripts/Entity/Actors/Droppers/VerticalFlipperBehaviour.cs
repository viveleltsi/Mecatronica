using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalFlipperBehaviour : MonoBehaviour
{

    #region Component links

    /// <summary>
    /// Flip animator
    /// </summary>
    [SerializeField]
    private Animator _animator;

    /// <summary>
    /// Container who rotate
    /// </summary>
    [SerializeField]
    private Transform _rotationContainer;

    /// <summary>
    /// Container for target to flip
    /// </summary>
    [SerializeField]
    private Transform _centerContainer;

    #endregion

    /// <summary>
    /// Target to flip
    /// </summary>
    private GameObject _target;

    /// <summary>
    /// Method to initialize the component
    /// Will move the VerticalFlipper on the right position
    /// and change the target object parent
    /// </summary>
    /// <param name="targetToFlip"></param>
    /// <param name="position"></param>
    public void AssignObject(GameObject targetToFlip, Vector3 position)
    {
        _target = targetToFlip;
        transform.position = position;
        var pivot = _target.transform.Find("RotationPivot");
        var center = _target.transform.Find("Center");
        InitPosition(pivot,center);
        _target.transform.SetParent(_centerContainer, false);
        _target.transform.localPosition = Vector3.zero;
        _target.transform.localRotation = Quaternion.identity;
    }

    public void StartFlipping()
    {
        _animator.SetTrigger("Fall");
    }

    private void InitPosition(Transform pivotPosition, Transform centerPosition)
    {
        var halfLenght = Mathf.Abs(pivotPosition.localPosition.x - centerPosition.localPosition.x);
        var halfHeight = centerPosition.localPosition.y - pivotPosition.localPosition.y;
        _rotationContainer.localPosition = Vector3.left * halfLenght;
        _centerContainer.localPosition = new Vector3(halfLenght,halfHeight,0f);
    }

}
