using Prototypes.Character;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterMovement))]
public class CharacterDeath : MonoBehaviour
{
    [Tooltip("Events that will happen on character's death.")]
    [SerializeField] private UnityEvent<Transform> onCharacterDeath;
    [SerializeField] private Transform mostRecentSpawnPoint;
    [Tooltip("Layers that will cause character's death.")]
    [SerializeField] private LayerMask deathLayers;
    [SerializeField] private bool isDebug;

    private void OnTriggerEnter(Collider other)
    {
        if ((deathLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            if (isDebug) SuperDebug.Log("I'm dead!");
            onCharacterDeath?.Invoke(transform);
            Respawn();
        }
    }
    
    /// <summary>
    /// Make sure that the player respawns on the most recently save SpawnPoint.
    /// </summary>
    private void Respawn()
    {
        Teleportation.Instance.Teleport(transform, mostRecentSpawnPoint);
        if (isDebug) SuperDebug.Log("respawn?");
    }

    /// <summary>
    /// Set the most recent SpawnPoint that the player should use.
    /// </summary>
    /// <param name="newSpawnPoint">The Transform where the player will respawn after death.</param>
    public void SetMostRecentSpawnPoint(Transform newSpawnPoint) => mostRecentSpawnPoint = newSpawnPoint;
}
