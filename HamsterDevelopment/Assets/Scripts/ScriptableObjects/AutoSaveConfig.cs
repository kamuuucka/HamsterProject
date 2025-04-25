using UnityEngine;

[CreateAssetMenu(fileName = "Auto-save Config", menuName = "Editor/Auto-save")]
public class AutoSaveConfig : ScriptableObject
{
    [Tooltip("Enable / Disable auto-saving.")]
    public bool enableAutoSaving = false;
    [Range(1,60)][Tooltip("Time between saves (in minutes).")]
    public int savingInterval = 15;
    [Tooltip("Show the debug message whenever the project is saved.")]
    public bool showDebugMessages = true;
}
