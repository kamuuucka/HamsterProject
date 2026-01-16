#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

public static class TagHelper
{
    public static string[] GetAllTags()
    {
        return InternalEditorUtility.tags;
    }
}
#endif