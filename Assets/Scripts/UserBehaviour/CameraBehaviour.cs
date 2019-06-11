using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    public CinemachineFreeLook FreeLook;

    public enum CameraMovementKind { Mouse, Wasd };

    public CameraMovementKind MovementKind;

    public float Speed = 10f;

    public float Threshold = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            FreeLook.m_XAxis.m_InputAxisName = "Mouse X";
            FreeLook.LookAt = transform;
        }
        else
        {
            FreeLook.m_XAxis.m_InputAxisValue = 0f;
            FreeLook.LookAt = null;
            FreeLook.m_XAxis.m_InputAxisName = "";

            switch (MovementKind)
            {
                case CameraMovementKind.Mouse:
                    HandleMouseMovement();
                    break;
                case CameraMovementKind.Wasd:
                    HandleWasdMovement();
                    break;
                default:
                    break;
            }
        }
       
    }

    private void HandleWasdMovement()
    {
        Vector3 cameraMovement = Vector3.zero;
        cameraMovement.x = Input.GetAxis("Horizontal");
        cameraMovement.z = Input.GetAxis("Vertical");

        if (cameraMovement.x != 1f && cameraMovement.x != -1f)
            cameraMovement.x = 0f;
        if (cameraMovement.z != 1f && cameraMovement.z != -1f)
            cameraMovement.z = 0f;
        if(cameraMovement != Vector3.zero)
        {
            var t = Camera.main.transform.right * cameraMovement.x + Camera.main.transform.forward * cameraMovement.z;
            t.y = 0f;
            transform.position = transform.position + t.normalized * Time.deltaTime * Speed;
        }
    }

    private void HandleMouseMovement()
    {
        var screenPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        float xCamera = screenPos.x;
        float yCamera = screenPos.y;
        Vector3 cameraMovement = Vector3.zero;
        if (xCamera < Threshold)
            cameraMovement.x = -1f;
        else if (xCamera > 1f - Threshold)
            cameraMovement.x = 1f;
        if (yCamera < Threshold)
            cameraMovement.z = -1f;
        else if (yCamera > 1f - Threshold)
            cameraMovement.z = 1f;

        transform.position = transform.position + cameraMovement.normalized * Time.deltaTime * Speed;
    }
}
