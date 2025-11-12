using UnityEngine;

/// <summary>
/// Sets a new rotation for the object.
/// </summary>
public class AdjustRotation : MonoBehaviour
{
    [Tooltip("New rotation for the object.")]
    [SerializeField] private Quaternion newRotation;

    /// <summary>
    /// Sets the new local rotation. (Does take the parent rotation into the consideration.)
    /// </summary>
    public void SetNewLocalRotation()
    {
        transform.localRotation = newRotation;
    }

    /// <summary>
    /// Sets the new global rotation. (Does NOT take the parent rotation into the consideration.)
    /// </summary>
    public void SetNewRotation()
    {
        transform.rotation = newRotation;
    }
    
    
}
