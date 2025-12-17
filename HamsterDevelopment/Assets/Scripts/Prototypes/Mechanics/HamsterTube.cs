using UnityEngine;

public class HamsterTube : MonoBehaviour
{
    public bool isDebug;
    private Transform _objectToTeleport;
    private Transform _destination;
    private bool _useRotation = false;
    
    public void GetObjectToTeleport(Transform objectToTeleport)
    {
        _objectToTeleport = objectToTeleport;
    }

    public void GetDestination(Transform destination)
    {
        _destination = destination;
    }

    public void ShouldUseRotation()
    {
        _useRotation = true;
    }

    public void TeleportToDestination()
    {
        Teleportation.Instance.Teleport(_objectToTeleport, _destination, _useRotation, isDebug);
    }
}
