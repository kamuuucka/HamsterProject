using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// The script responsible for teleporting specific objects to specific destinations. 
/// </summary>
public class Teleportation : MonoBehaviour
{
    public static Teleportation Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            SuperDebug.LogError("Another instance of Teleportation already exists!");
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Teleport(Transform objectToTeleport, Transform destination, bool useRotation = false, bool isDebug = false)
    {
        if (destination == null)
        {
            Debug.LogError("Destination not set!");
            return;
        }

        StartCoroutine(TeleportAfterFrame(objectToTeleport, destination, useRotation, isDebug));
    }

    /// <summary>
    /// Teleport after frame to avoid it not working :)
    /// </summary>
    private IEnumerator TeleportAfterFrame(Transform objectToTeleport, Transform destination, bool useRotation, bool isDebug)
    {
        // Disable physics/controllers interfering
        var controller = objectToTeleport.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        yield return new WaitForEndOfFrame(); // Wait for all updates

        // Apply teleport
        objectToTeleport.position = destination.position;
        if (useRotation) objectToTeleport.rotation = destination.rotation;

        if (isDebug) 
            SuperDebug.Log($"Teleported {objectToTeleport} to {destination.position}");

        // Re-enable components
        if (controller != null) controller.enabled = true;
    }
}