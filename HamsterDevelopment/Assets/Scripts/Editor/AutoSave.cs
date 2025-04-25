#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

[InitializeOnLoad]
public class AutoSave
{
    private static AutoSaveConfig _config;
    private static DateTime _lastSaveTime;
    private const string ConfigPath = "Assets/ScriptableObjects/AutoSaveConfig/AutoSaveConfig.asset";
    
    static AutoSave()
    {
        LoadConfig();
        EditorApplication.update += Update;
        _lastSaveTime = DateTime.Now;
    }

    /// <summary>
    /// Loads the config file or creates new one if missing.
    /// </summary>
    private static void LoadConfig()
    {
        _config = AssetDatabase.LoadAssetAtPath<AutoSaveConfig>(ConfigPath);
        if (_config == null)
        {
            _config = ScriptableObject.CreateInstance<AutoSaveConfig>();
            AssetDatabase.CreateAsset(_config, ConfigPath);
            AssetDatabase.SaveAssets();
            Debug.Log("Created new AutoSaveConfig at " + ConfigPath);
        }
    }

    private static void Update()
    {
        if (!_config.enableAutoSaving) return;

        TimeSpan timeSinceLastSave = DateTime.Now - _lastSaveTime;
        if (timeSinceLastSave.TotalMinutes >= _config.savingInterval)
        {
            SaveProject();
            ResetTime();
        }
    }

    /// <summary>
    /// Save the assets and the project.
    /// </summary>
    private static void SaveProject()
    {
        if (_config.showDebugMessages)
            Debug.Log("Auto-saving project at " + DateTime.Now.ToString("HH:mm:ss"));

        AssetDatabase.SaveAssets();
        EditorApplication.ExecuteMenuItem("File/Save Project");

        if (_config.showDebugMessages)
            Debug.Log("Auto-save completed");
    }

    /// <summary>
    /// Reset timer to the Interval time.
    /// </summary>
    private static void ResetTime()
    {
        _lastSaveTime = DateTime.Now;
    }

    /// <summary>
    /// Shortcut to see the config file.
    /// </summary>
    [MenuItem("Tools/AutoSave/Configure")]
    private static void ShowConfig()
    {
        Selection.activeObject = _config;
    }

    /// <summary>
    /// Force save with the reset.
    /// </summary>
    [MenuItem("Tools/AutoSave/Force Save Now")]
    private static void ForceSave()
    {
        SaveProject();
        ResetTime();
    }
}

#endif
