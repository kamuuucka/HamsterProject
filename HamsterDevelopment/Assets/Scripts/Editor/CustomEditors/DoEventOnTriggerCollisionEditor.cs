using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoEventOnTriggerCollision))]
public class DoEventOnTriggerCollisionEditor : Editor
{
    private SerializedProperty _onButtonInteractionProperty;
    private SerializedProperty _buttonToUseProperty;
    private SerializedProperty _isDebugProperty;

    private void OnEnable()
    {
        // Get the SerializedProperties once (more efficient)
        _onButtonInteractionProperty = serializedObject.FindProperty("onButtonInteraction");
        _buttonToUseProperty = serializedObject.FindProperty("buttonToUse");
        _isDebugProperty = serializedObject.FindProperty("isDebug");
    }

    public override void OnInspectorGUI()
    {
        // Draw the default inspector (non-hidden fields)
        base.OnInspectorGUI();

        // Update the SerializedObject
        serializedObject.Update();

        // Only show 'buttonToUse' if 'onButtonInteraction' is true
        if (_onButtonInteractionProperty.boolValue)
        {
            EditorGUILayout.PropertyField(_buttonToUseProperty, new GUIContent("Button To Use"));
        }

        EditorGUILayout.PropertyField(_isDebugProperty, new GUIContent("isDebug"));

        // Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}