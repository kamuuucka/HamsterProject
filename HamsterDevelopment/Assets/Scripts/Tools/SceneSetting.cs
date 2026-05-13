using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneSetting", menuName = "Scriptable Objects/SceneSetting")]
public class SceneSetting : ScriptableObject
{
    public List<SceneSettingData> Scenes = new List<SceneSettingData>();

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (Scenes.Count > 0)
        {
            //TODO: Check if all scenes are in build settings

            if (Scenes.Count(s => s.IsActiveScene) > 1)
            {
                Debug.LogWarning($"{this.name}: This Scenes setting has more than one active scene.");
            }
            
        }
    }
    #endif
}

[Serializable]
public class SceneSettingData
{
    [Scene] public string Scene;
    [Tooltip("Make sure that only one scene is active. If none are, only the first one will be considered the active scene")]public bool IsActiveScene = false;
}