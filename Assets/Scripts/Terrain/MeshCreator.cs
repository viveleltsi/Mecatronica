using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handle the creation of the mesh
/// </summary>
public static class MeshCreator
{
    /// <summary>
    /// Create a triangle
    /// </summary>
    /// <param name="data"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    public static void CreateTriangle(this MeshData data, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        int index = data.Vertices.Count;
        data.Vertices.Add(p1);
        data.Vertices.Add(p2);
        data.Vertices.Add(p3);
        data.Triangles.Add(index);
        data.Triangles.Add(index+1);
        data.Triangles.Add(index+2);
    }

    /// <summary>
    /// Create a triangle who facing the same direction as the facingDirectionVector
    /// </summary>
    /// <param name="data"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="facingDirection">Normal vector from surface to the view for visibility</param>
    public static void CreateTriangle(this MeshData data, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 facingDirection)
    {
        var dir = Vector3.Cross(
            p2 - p1,
            p3 - p1
            ).normalized;
        if(dir == facingDirection.normalized)
        {
            data.CreateTriangle(p1, p2, p3);
        }
        else
        {
            data.CreateTriangle(p1, p3, p2);
        }
    }

    /// <summary>
    /// Create a triangle with a specific height (Y axis)
    /// </summary>
    /// <param name="data"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="height"></param>
    /// <param name="facingDirection"></param>
    public static void CreateTriangleAtHeight(this MeshData data, Vector3 p1, Vector3 p2,Vector3 p3, float height, Vector3 facingDirection)
    {
        p1.y = p2.y = p3.y = height;
        data.CreateTriangle(p1, p2, p3, facingDirection);
    }

    /// <summary>
    /// Create a quad
    /// </summary>
    /// <param name="data"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    public static void CreateQuad(this MeshData data, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        //First triangle
        int index = data.Vertices.Count;
        data.Vertices.Add(p1);
        data.Vertices.Add(p3);
        data.Vertices.Add(p2);
        data.Vertices.Add(p4);

        data.Triangles.Add(index);
        data.Triangles.Add(index + 1);
        data.Triangles.Add(index + 2);

        //Second triangle
        data.Triangles.Add(index);
        data.Triangles.Add(index + 3);
        data.Triangles.Add(index + 1);
    }

    /// <summary>
    /// Create a quad with a specific facing direction
    /// </summary>
    /// <param name="data"></param>
    /// <param name="quadPoints"></param>
    /// <param name="facingDirection"></param>
    /// <param name="height"></param>
    public static void CreateQuad(this MeshData data, List<Vector3> quadPoints, Vector3 facingDirection, float? height = null)
    {
        var p1 = quadPoints[0];
        var p2 = quadPoints[1];
        var p3 = quadPoints[2];
        var p4 = quadPoints[3];
        if (height != null)
        {
            p1.y = p2.y = p3.y = p4.y = height.Value;
        }
        var actualDirection = Vector3.Cross(p2 - p1, p3 - p1).normalized;
        if (actualDirection != facingDirection)
        {
            data.CreateQuad(p1, p2, p3, p4);
        }
        else
        {
            data.CreateQuad(p1, p4, p3, p2);
        }
    }


    /// <summary>
    /// New method to construct a quad
    /// </summary>
    /// <param name="data"></param>
    /// <param name="points"></param>
    /// <param name="quadPosition"></param>
    /// <returns>true if quad was created, false if face was not created</returns>
    public static bool CreateSquareStep(this MeshData data, List<Vector3> points, Vector2Int quadPosition)
    {
        var groups = points.GroupBy(p => p.y).ToDictionary(c => c.Key, c => c.ToList());
        var lowerPoint = points.OrderBy(p => p.y).FirstOrDefault();

        // how is the ground ? quad or 2 triangles?
        // If there's a diagonal it'll be 2 triangles
        var firstDiagonalPoint = points.FirstOrDefault(p => p.x == lowerPoint.x && p.z != lowerPoint.z);
        var secondDiagonalPoint = points.FirstOrDefault(p => p.x != lowerPoint.x && p.z == lowerPoint.z);
        var lastPoint = points.FirstOrDefault(x => x != firstDiagonalPoint &&
                x != secondDiagonalPoint && x != lowerPoint);
        if (firstDiagonalPoint.y > lowerPoint.y &&
            secondDiagonalPoint.y > lowerPoint.y
            && lastPoint.y != lowerPoint.y)
        {
            //We have a diagonal we make a first triangle
            data.CreateTriangleAtHeight(lowerPoint, firstDiagonalPoint, secondDiagonalPoint,lowerPoint.y, Vector3.up);
            //We create a second triangle at the lower height on the diagonales points
            var secondTriangleHeight = Mathf.Min(firstDiagonalPoint.y, secondDiagonalPoint.y);
            data.CreateTriangleAtHeight(firstDiagonalPoint, secondDiagonalPoint, lastPoint, secondTriangleHeight, Vector3.up);
            //And we create the face between first triangle and second one
            var p1 = firstDiagonalPoint;
            var p2 = secondDiagonalPoint;
            var p3 = secondDiagonalPoint;
            var p4 = firstDiagonalPoint;
            p1.y = p2.y = secondTriangleHeight;
            p3.y = p4.y = lowerPoint.y;
            var facingDirection = (lowerPoint - (p3+p4) / 2f).normalized;
            data.CreateQuad(new List<Vector3> { p1, p2, p3, p4 }, facingDirection);
            // Other point need to be checked for border face
            if (lastPoint.y > secondTriangleHeight)
            {
                if(firstDiagonalPoint.y > secondTriangleHeight)
                {
                    data.CreateFaceIfNeeded(firstDiagonalPoint, lastPoint, secondTriangleHeight, points);
                }
                if(secondDiagonalPoint.y > secondTriangleHeight)
                {
                    data.CreateFaceIfNeeded(secondDiagonalPoint, lastPoint, secondTriangleHeight, points);
                }
            }
        }
        else
        {
            
            //We dont have a diagonal so we make a square
            
            //We need to check all face
            var p1 = points.FirstOrDefault();
            var p2 = points.FirstOrDefault(p => p.x == p1.x && p != p1);
            var p3 = points.FirstOrDefault(p => p.z == p2.z && p != p2);
            var p4 = points.FirstOrDefault(p => p.x == p3.x && p != p3);
            bool faceCreated = false;
            faceCreated |= data.CreateFaceIfNeeded(p1, p2, lowerPoint.y, points);
            faceCreated |= data.CreateFaceIfNeeded(p2, p3, lowerPoint.y, points);
            faceCreated |= data.CreateFaceIfNeeded(p3, p4, lowerPoint.y, points);
            faceCreated |= data.CreateFaceIfNeeded(p4, p1, lowerPoint.y, points);
            if (faceCreated)
            {
                data.CreateQuad(points, Vector3.up, lowerPoint.y);
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Create face if needed (for square points)
    /// </summary>
    /// <param name="data"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="groundHeight"></param>
    /// <param name="allPoints"></param>
    public static bool CreateFaceIfNeeded(this MeshData data, Vector3 p1, Vector3 p2, float groundHeight,List<Vector3> allPoints)
    {
        if(p1.y > groundHeight && p2.y > groundHeight)
        {
            var height = Mathf.Min(p1.y, p2.y);
            var otherPoint = allPoints.FirstOrDefault(p => p.x != p1.x && p.z != p1.z);
            otherPoint.y = p2.y;
            var facingDirection = (otherPoint - p2).normalized;
            var p3 = p2;
            var p4 = p1;
            p1.y = p2.y = height;
            p3.y = p4.y = groundHeight;
            data.CreateQuad(new List<Vector3>() { p1, p2, p3, p4 }, facingDirection);
            return true;
        }
        else
        {
            return false;
        }
    }

}
