using System;
using Prototypes.Character;
using Unity.VisualScripting;
using UnityEngine;

public class HamsterWheel : MonoBehaviour
{
    [SerializeField] private Transform attachPoint;
    [SerializeField] private GameObject miniGameOverlay;

    private void Start()
    {
        miniGameOverlay.SetActive(false);
    }

    /// <summary>
    /// Attaches the object with the CharacterMovement script to the AttachPoint.
    /// </summary>
    /// <param name="player">Object that has the Character Movement script.</param>
    public void AttachToWheel(CharacterMovement player)
    {
        player.transform.position = attachPoint.position;
        player.transform.localRotation = attachPoint.localRotation;
        player.GetCharacterController().enabled = false;
    }
}
