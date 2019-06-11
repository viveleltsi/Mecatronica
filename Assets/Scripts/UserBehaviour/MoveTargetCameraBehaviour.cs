using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTargetCameraBehaviour : MonoBehaviour
{
    /// <summary>
    /// Move speed of the point that the camera look up
    /// </summary>
    public float MoveSpeed = 10f;

    /// <summary>
    /// Link to the main camera
    /// </summary>
    [SerializeField]
    private Camera _camera;

    /// <summary>
    /// View direction when start moving (target position - camera position)
    /// (To avoid trouble with the Cinemachine effect (small rotation)
    /// </summary>
    private Vector3? _forwardDirectionWhenStart = null;

    // Update is called once per frame
    void FixedUpdate()
    {
        HandlePosition();
    }

    /// <summary>
    /// Handle the movement of the camera
    /// </summary>
    private void HandlePosition()
    {
        Vector2 inputAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if(inputAxis == Vector2.zero)
        {
            _forwardDirectionWhenStart = null;
        }
        else
        {
            if (_forwardDirectionWhenStart == null)
            {
                var dir = (transform.position - _camera.transform.position).normalized;
                dir.y = 0f;
                _forwardDirectionWhenStart = dir;
            }
            var cameraRight = new Vector3(_forwardDirectionWhenStart.Value.z, 0f, -_forwardDirectionWhenStart.Value.x);
            var realDirection = (_forwardDirectionWhenStart.Value * inputAxis.y + cameraRight * inputAxis.x).normalized;
            transform.position = transform.position + realDirection * MoveSpeed * Time.fixedDeltaTime;
        }
    }
}
