using UnityEngine;

public class CursorPosition
{
    public CursorPosition() { }

    /// <summary>
    /// Distance of the raycast
    /// </summary>
    private float _rayCastLimit = 1000000f;

    /// <summary>
    /// Plan when raycasting the world position
    /// </summary>
    private Plane _worldPlane = new Plane(Vector3.up, Vector3.zero);

    /// <summary>
    /// Position projected to the ground
    /// </summary>
    public Vector3 GroundPosition
    {
        get
        {
            return GetGroundPosition();
        }
    }

    /// <summary>
    /// Found the projected position on the ground
    /// </summary>
    /// <returns></returns>
    private Vector3 GetGroundPosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        var layer = LayerMask.GetMask(new string[] { "Ground" });
        if(Physics.Raycast(ray,out hit, _rayCastLimit, layer))
        {
            return ray.GetPoint(hit.distance);
        }
        else
        {
            float enter;
            _worldPlane.Raycast(ray, out enter);
            return ray.GetPoint(enter);
        }
    }
}

