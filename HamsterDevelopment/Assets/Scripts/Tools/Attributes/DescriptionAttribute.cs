using UnityEditor;
using UnityEngine;

// Works on both fields and classes
[System.AttributeUsage(
    System.AttributeTargets.Field | 
    System.AttributeTargets.Class,
    AllowMultiple = true)]
public class DescriptionAttribute : PropertyAttribute
{
    public readonly string Text;
    public MessageType messageType = MessageType.Info;
    
    public DescriptionAttribute(string text) => Text = text;
}