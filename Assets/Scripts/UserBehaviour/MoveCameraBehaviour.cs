using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraBehaviour : MonoBehaviour
{
    /// <summary>
    /// Camera to move
    /// </summary>
    public Camera Camera;

    public float MoveSpeed = 10f;

    public float ScrollSpeed = 1f;

    public float MaxZoom = 44f;

    public float MinZoom = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandlePosition();
        HandleZoom();
    }

    /// <summary>
    /// Handle the movement of the camera
    /// </summary>
    private void HandlePosition()
    {
        var direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (direction != Vector3.zero)
        {
            var zoomFactor = Camera.fieldOfView / (MaxZoom - MinZoom) + 0.2f;
            zoomFactor = Mathf.Clamp01(zoomFactor);
            Camera.transform.position = Camera.transform.position + direction * (MoveSpeed * Time.fixedDeltaTime * zoomFactor);
        }
    }

    /// <summary>
    /// Handle the camera zoom
    /// </summary>
    private void HandleZoom()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            var field = Camera.fieldOfView - scroll * ScrollSpeed;
            Camera.fieldOfView = Mathf.Clamp(field, MinZoom, MaxZoom);
        }
    }
}
