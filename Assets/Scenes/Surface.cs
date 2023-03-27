using System;
using UnityEngine;
using UnityEngine.Serialization;

public enum ShapeType
{
    Sphere,
    Cube,
    Plane,
}
public enum BlendMode
{
    Union,
    SmoothUnion,
    Subtraction,
    SmoothSubtraction,
    UnionSmoothSubtraction,
    Intersection,
    SmoothIntersection,
}



public class Surface : MonoBehaviour
{
    public Vector3 Position => transform.position;
    public Vector3 Rotation
    {
        get
        {
            return new Vector3(transform.rotation.eulerAngles.x * Mathf.Deg2Rad,
                transform.rotation.eulerAngles.y * Mathf.Deg2Rad, transform.rotation.eulerAngles.z * Mathf.Deg2Rad);
        }
    }

    public Vector3 Scale => transform.lossyScale;
    public ShapeType ShapeType;
    public BlendMode BlendMode;
    [Range(0,1)] public float BlendStrength;
    public Color Diffuse;

    public static bool DrawGizmos;


    private void OnDrawGizmos()
    {
       // Debug.Log((4 & 1) == 1);
        if (!DrawGizmos) return;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.5f,0.5f,0.5f));
    }
}