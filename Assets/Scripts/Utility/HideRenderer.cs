using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class HideRenderer : MonoBehaviour
{
    [SerializeField, ReadOnly] bool showMesh = false;
    
    public void ToggleRenderers()
    {
        showMesh = !showMesh;
        foreach (Renderer obj in transform.GetComponentsInChildren<Renderer>())
        {
            obj.enabled = showMesh;
        }
    }
}

[CustomEditor(typeof(HideRenderer))]
public class HideRendererEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        base.OnInspectorGUI();

        // Add a custom button in the Inspector
        if (GUILayout.Button("Toggle Renderers")) 
        {
            ((HideRenderer)target).ToggleRenderers();
        }
    }
}
#endif