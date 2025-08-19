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
        GameObject newObject = new GameObject(_myString);
        newObject.transform.position = Vector3.zero;
        GameObject model = new GameObject("Model");
        model.transform.position = Vector3.zero;
        model.transform.SetParent(newObject.transform);
        GameObject newPrimitive = null;
        switch (_index)
        {
            case 0:
                newPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
            case 1:
                newPrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case 2:
                newPrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case 3:
                newPrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case 4:
                newPrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case 5:
                newPrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            default:
                SuperDebug.Log("WRONG!");
                break;
        }

        if (newPrimitive != null)
        {
            newPrimitive.transform.position = Vector3.zero;
            newPrimitive.transform.SetParent(model.transform);
        }
      
    }
}