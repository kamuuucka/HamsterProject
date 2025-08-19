using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Class responsible for creating prefabs using the template used by the team.
/// </summary>
public class CustomPrefabTemplate : EditorWindow
{
    [MenuItem("GameObject/Prefab Templates/Empty", false, 0)]
    public static void CreateEmpty()
    {
        InstantiatePrefabTemplate();
    }

    [MenuItem("GameObject/Prefab Templates/Cube", false, 1)]
    public static void CreateCube()
    {
        InstantiatePrefab((PrimitiveType)Enum.Parse(typeof(PrimitiveType), "Cube"));
    }

    [MenuItem("GameObject/Prefab Templates/Sphere", false, 2)]
    public static void CreateSphere()
    {
        InstantiatePrefab((PrimitiveType)Enum.Parse(typeof(PrimitiveType), "Sphere"));
    }

    [MenuItem("GameObject/Prefab Templates/Capsule", false, 3)]
    public static void CreateCapsule()
    {
        InstantiatePrefab((PrimitiveType)Enum.Parse(typeof(PrimitiveType), "Capsule"));
    }

    [MenuItem("GameObject/Prefab Templates/Cylinder", false, 4)]
    public static void CreateCylinder()
    {
        InstantiatePrefab((PrimitiveType)Enum.Parse(typeof(PrimitiveType), "Cylinder"));
    }

    [MenuItem("GameObject/Prefab Templates/Plane", false, 5)]
    public static void CreatePlane()
    {
        InstantiatePrefab((PrimitiveType)Enum.Parse(typeof(PrimitiveType), "Plane"));
    }

    [MenuItem("GameObject/Prefab Templates/Quad", false, 6)]
    public static void CreateQuad()
    {
        InstantiatePrefab((PrimitiveType)Enum.Parse(typeof(PrimitiveType), "Quad"));
    }

    private static GameObject InstantiatePrefabTemplate(string name = "Prefab")
    {
        GameObject newObject = new GameObject($"Custom {name}");
        newObject.transform.position = Vector3.zero;
        GameObject model = new GameObject("Model");
        model.transform.position = Vector3.zero;
        model.transform.SetParent(newObject.transform);
        return model;
    }

    private static void InstantiatePrefab(PrimitiveType desiredPrimitive)
    {
        var model = InstantiatePrefabTemplate(desiredPrimitive.ToString());
        GameObject newPrimitive = GameObject.CreatePrimitive(desiredPrimitive);
        newPrimitive.transform.position = Vector3.zero;
        newPrimitive.transform.SetParent(model.transform);
    }
}