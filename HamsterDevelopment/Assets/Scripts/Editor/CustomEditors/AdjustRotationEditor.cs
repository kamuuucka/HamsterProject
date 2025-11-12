using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AdjustRotation))]
public class AdjustRotationEditor : Editor
{
    private AdjustRotation _adjustRotation;
    private void OnEnable()
    {
        _adjustRotation = (AdjustRotation)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Test Local Rotation"))
        {
            _adjustRotation.SetNewLocalRotation();
        }
        
        if (GUILayout.Button("Test Global Rotation"))
        {
            _adjustRotation.SetNewRotation();
        }

        if (GUILayout.Button("Reset"))
        {
            _adjustRotation.GoBackToOriginalRotation();
        }
    }
}
