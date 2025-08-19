using System;
using System.Reflection;
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
        FocusAndRename(InstantiatePrefabTemplate());
    }

    [MenuItem("GameObject/Prefab Templates/Cube", false, 1)]
    public static void CreateCube()
    {
        FocusAndRename(InstantiatePrefab((PrimitiveType)Enum.Parse(typeof(PrimitiveType), "Cube")));
    }

    [MenuItem("GameObject/Prefab Templates/Sphere", false, 2)]
    public static void CreateSphere()
    {
        FocusAndRename(InstantiatePrefab((PrimitiveType)Enum.Parse(typeof(PrimitiveType), "Sphere")));
    }

    [MenuItem("GameObject/Prefab Templates/Capsule", false, 3)]
    public static void CreateCapsule()
    {
       FocusAndRename(InstantiatePrefab((PrimitiveType)Enum.Parse(typeof(PrimitiveType), "Capsule")));
    }

    [MenuItem("GameObject/Prefab Templates/Cylinder", false, 4)]
    public static void CreateCylinder()
    {
        FocusAndRename(InstantiatePrefab((PrimitiveType)Enum.Parse(typeof(PrimitiveType), "Cylinder")));
    }

    [MenuItem("GameObject/Prefab Templates/Plane", false, 5)]
    public static void CreatePlane()
    {
        FocusAndRename(InstantiatePrefab((PrimitiveType)Enum.Parse(typeof(PrimitiveType), "Plane")));
    }

    [MenuItem("GameObject/Prefab Templates/Quad", false, 6)]
    public static void CreateQuad()
    {
        FocusAndRename(InstantiatePrefab((PrimitiveType)Enum.Parse(typeof(PrimitiveType), "Quad")));
    }

    private static GameObject InstantiatePrefabTemplate(string name = "Prefab")
    {
        GameObject newObject = new GameObject($"Custom {name}");
        newObject.transform.position = Vector3.zero;
        GameObject model = new GameObject("Model");
        model.transform.position = Vector3.zero;
        model.transform.SetParent(newObject.transform);
        return newObject;
    }

    private static GameObject InstantiatePrefab(PrimitiveType desiredPrimitive)
    {
        GameObject newObject = InstantiatePrefabTemplate(desiredPrimitive.ToString());
        GameObject model = newObject.transform.Find("Model").gameObject;
        GameObject newPrimitive = GameObject.CreatePrimitive(desiredPrimitive);
        Undo.RegisterCreatedObjectUndo(newPrimitive, "Create Primitive");
        newPrimitive.transform.position = Vector3.zero;
        newPrimitive.transform.SetParent(model.transform);
        return newObject;
    }

    private static void FocusAndRename(GameObject go)
    {
        Selection.activeGameObject = go;
        
        EditorApplication.delayCall += () =>
        {
            if (Selection.activeGameObject == go)
            {
                var hierarchyType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
                var window = EditorWindow.GetWindow(hierarchyType);
                var renameMethod = hierarchyType.GetMethod("RenameGO", BindingFlags.Instance | BindingFlags.NonPublic);
                renameMethod?.Invoke(window, null);
            }
        };
    }
}