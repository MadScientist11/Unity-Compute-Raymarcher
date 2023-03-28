using UnityEngine;

public enum ShapeType
{
    Sphere = 0,
    Cube = 1,
    Torus = 2,
    Plane = 3,
    Cylinder = 4,
}
public enum BlendMode
{
    Union = 0,
    SmoothUnion = 1,
    Subtraction = 2,
    SmoothSubtraction = 3,
    UnionSmoothSubtraction = 4,
    Intersection = 5,
    SmoothIntersection = 6,
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
        if (!DrawGizmos) return;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.5f,0.5f,0.5f));
    }
}