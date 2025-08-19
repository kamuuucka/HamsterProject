using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MyWindow : EditorWindow
{
    private string _myString = "NewPrefab";
    private int _index = 0;
    private string[] _options = {"Cube", "Sphere", "Capsule", "Cylinder", "Plane", "Quad"};
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    [MenuItem("Tools/Create Prefab")]
    public static void ShowWindow()
    {
        GetWindow(typeof(MyWindow));
    }

    private void OnGUI()
    {
        GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
        _myString = EditorGUILayout.TextField ("Prefab Name:", _myString);
        _index = EditorGUILayout.Popup("Prefab Shape:", _index, _options);
        if (GUILayout.Button("Create"))
        {
            InstantiatePrefab();
        }

        // groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
        // myBool = EditorGUILayout.Toggle ("Toggle", myBool);
        // myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
        // EditorGUILayout.EndToggleGroup ();
    }

    private void InstantiatePrefab()
    {
        switch (_index)
        {
            case 0:
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = Vector3.zero;
                break;
            }
            default:
                SuperDebug.Log("WRONG!");
                break;
        }
    }
}