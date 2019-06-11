using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraViewPoints
{
    /// <summary>
    /// Min point in world space
    /// </summary>
    public Vector3 MinPoint;

    /// <summary>
    /// Max point in world space
    /// </summary>
    public Vector3 MaxPoint;

    /// <summary>
    /// Viewport points to check
    /// </summary>
    private readonly List<Vector3> _viewportPoints = new List<Vector3>()
    {
        new Vector3(0,0,0),
        new Vector3(0,1,0),
        new Vector3(1,0,0),
        new Vector3(1,1,0)
    };

    /// <summary>
    /// central point of field of view
    /// </summary>
    private readonly Vector3 _centralPoints = new Vector3(0.5f, 0.5f, 0f);

    /// <summary>
    /// Ground layer
    /// </summary>
    private readonly Plane _worldPlane = new Plane(Vector3.up, Vector3.zero);

    /// <summary>
    /// Max distance for raycasting
    /// </summary>
    private const float MaxDistance = 10000f;

    /// <summary>
    /// Check the boundary of the camera to find min and max points
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="maxViewDistance"></param>
    public void CheckBoundary(Camera camera, float maxViewDistance)
    {
        //Find min and max
        List<Vector3> points = new List<Vector3>();
        foreach (var point in _viewportPoints)
        {
           
            var ray = camera.ViewportPointToRay(point);
            float enter = 0f;
            if(_worldPlane.Raycast(ray, out enter))
            {
                points.Add(ray.GetPoint(enter));
            }
        }
        if(points != null && points.Count > 0)
        {
            MinPoint = new Vector3(points.Min(p => p.x), 0f, points.Min(p => p.z));
            MaxPoint = new Vector3(points.Max(p => p.x), 0f, points.Max(p => p.z));
            //Check if the point are at correct distance otherwise correct them
            var projectedPositionOnPlane =  camera.transform.position;
            projectedPositionOnPlane.y = 0f;
            MinPoint = CheckViewDistance(MinPoint, projectedPositionOnPlane, maxViewDistance);
            MaxPoint = CheckViewDistance(MaxPoint, projectedPositionOnPlane, maxViewDistance);
           
        }
    }

    /// <summary>
    /// Check the view distance and give a corrected vector
    /// </summary>
    /// <param name="point"></param>
    /// <param name="origin"></param>
    /// <param name="maxDistance"></param>
    /// <returns></returns>
    private Vector3 CheckViewDistance(Vector3 point,Vector3 origin, float maxDistance)
    {
        var segment = point - origin;
        if (segment.sqrMagnitude > maxDistance * maxDistance)
        {
            //Debug.Log($"Segment to big {segment.sqrMagnitude} / {maxDistance * maxDistance}");
            var dir = segment.normalized;
            point = origin + segment.normalized * maxDistance;
        }
        return point;
    }
}
