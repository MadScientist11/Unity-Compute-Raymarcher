using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
 
[ExecuteInEditMode]
public class SceneViewCameraProxy : MonoBehaviour
{
    #if UNITY_EDITOR
    public SceneView SceneView;
    public Camera Camera;
 
    public Camera ReferenceCamera;
    public bool UseReferenceCamera;
 
    public void OnEnable()
    {
        Camera = GetCamera();
        UpdateComponents();
    }
 
    private Camera GetCamera()
    {
        SceneView = EditorWindow.GetWindow<SceneView>();
        return SceneView.camera;
    }
 
    private Component[] GetComponents()
    {
        var result = UseReferenceCamera
            ? ReferenceCamera.GetComponents<Component>()
            : GetComponents<Component>();
 
        if (result != null && result.Length > 1) // Exclude Transform
        {
            result = result.Except(new[] {UseReferenceCamera ? ReferenceCamera.transform : transform}).ToArray();
 
            var hasCamera = UseReferenceCamera ? true : GetComponent<Camera>() != null;
            if (hasCamera)
                result = UseReferenceCamera ? result.Except(new[] {ReferenceCamera}).ToArray() : result.Except(new[] {GetComponent<Camera>()}).ToArray();
        }
 
        return result;
    }
 
    private void UpdateComponents()
    {
        if(Camera == null)
            Camera = GetCamera();
 
        if (Camera == null) // This shouldn't happen, but it does
            return;
 
        if(UseReferenceCamera && ReferenceCamera == null)
            throw new Exception("UseReferenceCamera enabled, but none chosen.");
 
        var components = GetComponents();
        if (components != null && components.Length > 1)
        {
            var cameraGo = Camera.gameObject;
 
            Debug.Log(cameraGo);
            Debug.Log(cameraGo.GetComponents(typeof(Component)).Length);
 
            for (var i = 0; i < components.Length; i++)
            {
                var c = components[i];
                var cType = c.GetType();
 
                var existing = cameraGo.GetComponent(cType) ?? cameraGo.AddComponent(cType);
 
                EditorUtility.CopySerialized(c, existing);
            }
        }
    }
    #endif
}