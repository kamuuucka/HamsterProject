#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DescriptionAttribute))]
public class DescriptionDrawer : DecoratorDrawer
{
    public override float GetHeight()
    {
        var attr = (DescriptionAttribute)attribute;
        return EditorStyles.helpBox.CalcHeight(
            new GUIContent(attr.Text), 
            EditorGUIUtility.currentViewWidth - 30
        ) + EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position)
    {
        var attr = (DescriptionAttribute)attribute;
        position.height -= EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.HelpBox(position, attr.Text, attr.messageType);
    }
}
#endif