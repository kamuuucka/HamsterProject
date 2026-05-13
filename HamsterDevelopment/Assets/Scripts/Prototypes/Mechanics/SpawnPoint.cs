using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Tooltip("Show the debug things. For example the gizmo sphere to easier locate the object.")]
    [SerializeField] private bool isDebug = true;
    
    private void OnDrawGizmosSelected()
    {
        if (!isDebug) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }

}
