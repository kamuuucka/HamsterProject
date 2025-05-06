using System.Collections;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    [SerializeField] private bool isDebug;
    private Transform _destination;

    public void SetDestination(Transform destination)
    {
        _destination = destination;
        if (isDebug) SuperDebug.Log($"{_destination}");
    }

    private void AssignNewPosition(Transform objectToTeleport)
    {
        objectToTeleport.position = _destination.position;
        if (isDebug) SuperDebug.Log($"Teleporting {objectToTeleport} to {_destination.position}!");
    }
    
    public void Teleport(Transform objectToTeleport)
    {
        StartCoroutine(TeleportAfterFrame(objectToTeleport));
    }

    private IEnumerator TeleportAfterFrame(Transform objectToTeleport)
    {
        yield return null; // Wait one frame
        AssignNewPosition(objectToTeleport);
    }
}
