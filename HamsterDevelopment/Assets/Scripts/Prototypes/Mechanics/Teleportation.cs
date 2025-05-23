using System.Collections;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    [SerializeField] private bool isDebug;
    private Transform _destination;

    public void SetDestination(Transform destination) => _destination = destination;

    public void Teleport(Transform objectToTeleport)
    {
        if (_destination == null)
        {
            Debug.LogError("Destination not set!");
            return;
        }

        StartCoroutine(TeleportAfterFrame(objectToTeleport));
    }

    private IEnumerator TeleportAfterFrame(Transform objectToTeleport)
    {
        // Disable physics/controllers interfering
        var controller = objectToTeleport.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        yield return new WaitForEndOfFrame(); // Wait for all updates

        // Apply teleport
        objectToTeleport.position = _destination.position;
        objectToTeleport.rotation = _destination.rotation;

        if (isDebug) 
            SuperDebug.Log($"Teleported {objectToTeleport} to {_destination.position}");

        // Re-enable components
        if (controller != null) controller.enabled = true;
    }
}