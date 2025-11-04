using UnityEngine;

public class AdjustRotation : MonoBehaviour
{
    [SerializeField] private Quaternion newRotation;

    public void SetNewRotation()
    {
        transform.rotation = newRotation;
    }
}
