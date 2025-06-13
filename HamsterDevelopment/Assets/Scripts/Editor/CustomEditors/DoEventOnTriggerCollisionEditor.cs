using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoEventOnTriggerCollision))]
public class DoEventOnTriggerCollisionEditor : Editor
{
    private SerializedProperty _onButtonInteractionProperty;
    private SerializedProperty _buttonToUseProperty;
    private SerializedProperty _isDebugProperty;
    private SerializedProperty _selectedTags;

    private void OnEnable()
    {
        _onButtonInteractionProperty = serializedObject.FindProperty("onButtonInteraction");
        _buttonToUseProperty = serializedObject.FindProperty("buttonToUse");
        _isDebugProperty = serializedObject.FindProperty("isDebug");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        serializedObject.Update();
        
        if (_onButtonInteractionProperty.boolValue)
        {
            EditorGUILayout.PropertyField(_buttonToUseProperty, new GUIContent("Button To Use"));
        }

        EditorGUILayout.PropertyField(_isDebugProperty, new GUIContent("isDebug"));
        
        serializedObject.ApplyModifiedProperties();
    }
}