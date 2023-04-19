using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public struct SurfaceData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public Vector3 diffuse;
    public int shapeType;
    public int blend;
    public int operations;
    public int operationsCount;
    public float blendStrength;

    public static int GetSize()
    {
        return sizeof(float) * 13 + sizeof(int) * 4;
    }
}

public static class VectorExtensions
{
    public static Vector3 ToVector3(this Vector4 vec)
    {
        return new Vector3(vec.x, vec.y, vec.z);
    }
}
[Flags]
public enum Operation
{
    None = 0,
    Repeat = 1,
    Round = 2,
    Onion = 4,
    FBM = 8,
    
}
public interface IOperation
{
    public Operation OperationId { get; set; }
    public Vector4 Value { get; }
}


[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Raymarcher : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private ComputeShader _raymarchingCS;
    [SerializeField] private Transform _light;
    [SerializeField] private bool _drawGizmos;
    
    [Header("Raymarching settings")]
    [SerializeField] private bool _fog;
    [SerializeField] private bool _outline;
    
    
    

    private RenderTexture _renderTexture;
    private int _kernelIndex = 1;
    private Color _lightColor;

    private List<ComputeBuffer> _surfacesData;

    private void OnValidate()
    {
        Surface.DrawGizmos = _drawGizmos;
    }


    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        _camera = Camera.current;
        _kernelIndex = _raymarchingCS.FindKernel("CSMain");
        _lightColor = _light.GetComponent<Light>().color;
        _surfacesData = new List<ComputeBuffer>();


        InitRenderTexture();
        SetShapesRenderData();

        _raymarchingCS.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        _raymarchingCS.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
        _raymarchingCS.SetVector("_CameraForward", _camera.transform.forward);
        _raymarchingCS.SetVector("_LightDirection", _light.forward);
        _raymarchingCS.SetVector("_LightColor", _lightColor);
        _raymarchingCS.SetVector("_LightPos", _light.position);
        _raymarchingCS.SetFloat("_Time", (float)EditorApplication.timeSinceStartup);
        _raymarchingCS.SetBool("_Fog", _fog);
        _raymarchingCS.SetBool("_Outline", _outline);

        _raymarchingCS.SetTexture(_kernelIndex, "Source", src);
        _raymarchingCS.SetTexture(_kernelIndex, "Destination", _renderTexture);

        DispatchShader();
        Render(src, dest);

        foreach (var buffer in _surfacesData)
        {
            buffer?.Dispose();
        }
    }

    private void Render(RenderTexture src, RenderTexture dest)
    {
        if (FindObjectsOfType<Surface>().OrderBy(x => x.BlendMode).ToList().Count == 0)
        {
            Graphics.Blit(src, dest);
            return;
        }

        Graphics.Blit(_renderTexture, dest);
    }

    private void DispatchShader()
    {
        _raymarchingCS.GetKernelThreadGroupSizes(_kernelIndex, out uint x, out uint y, out _);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / x);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / y);
        _raymarchingCS.Dispatch(_kernelIndex, threadGroupsX, threadGroupsY, 1);
    }

    private void SetShapesRenderData()
    {
        List<Surface> surfaces = FindObjectsOfType<Surface>().OrderBy(x => x.BlendMode).ToList();
        SurfaceData[] shapeData = new SurfaceData[surfaces.Count];
        List<Vector4> operationValues = new List<Vector4>();
        for (int i = 0; i < surfaces.Count; i++)
        {
            Surface surface = surfaces[i];
            List<IOperation> operations =
                surface.GetComponents<IOperation>().OrderBy(x => x.OperationId).ToList();
            operationValues.AddRange(operations.Select(x => x.Value).ToList());

            int operationFlags = 0;

            if (operations is { Count: > 0 })
            {
                operationFlags =
                    (int)operations.Select(x => x.OperationId).Aggregate((current, next) => current | next);
            }

            shapeData[i] = new SurfaceData()
            {
                position = surface.Position,
                scale = surface.Scale,
                rotation = surface.Rotation,
                shapeType = (int)surface.ShapeType,
                blend = (int)surface.BlendMode,
                operations = operationFlags,
                operationsCount = operations.Count,
                diffuse = ((Vector4)surface.Diffuse).ToVector3(),
                blendStrength = surface.BlendStrength,
            };
        }

        ComputeBuffer shapeBuffer = null;
        if (shapeData.Length != 0)
        {
            shapeBuffer = new ComputeBuffer(shapeData.Length, SurfaceData.GetSize());
            shapeBuffer.SetData(shapeData);
            _raymarchingCS.SetBuffer(_kernelIndex, "shapes", shapeBuffer);
        }


        _raymarchingCS.SetInt("numShapes", shapeData.Length);

        ComputeBuffer operationValuesBuffer = null;
        if (operationValues.Count != 0)
        {
            operationValuesBuffer =
                new ComputeBuffer(operationValues.Count, sizeof(float) * 4 * operationValues.Count);
            operationValuesBuffer.SetData(operationValues.ToArray());
            _raymarchingCS.SetBuffer(_kernelIndex, "operationValues", operationValuesBuffer);
        }
        else
        {
            operationValuesBuffer =
                new ComputeBuffer(1, sizeof(float) * 4);
            operationValuesBuffer.SetData(new[] { Vector4.one * -1 });
            _raymarchingCS.SetBuffer(_kernelIndex, "operationValues", operationValuesBuffer);
        }

        _surfacesData.Add(shapeBuffer);
        _surfacesData.Add(operationValuesBuffer);
    }

    private void InitRenderTexture()
    {
        if (_renderTexture == null || _renderTexture.width != _camera.pixelWidth ||
            _renderTexture.height != _camera.pixelHeight)
        {
            if (_renderTexture != null)
            {
                _renderTexture.Release();
            }

            _renderTexture = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _renderTexture.enableRandomWrite = true;
            _renderTexture.Create();
        }
    }
}