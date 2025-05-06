using UnityEngine;

public class DebugLogText : MonoBehaviour
{
    [TextArea(10,20)][Tooltip("Text that should be displayed as debug")]
    [SerializeField] private string text;

    public void ShowText()
    {
        SuperDebug.Log(text);
    }
}
