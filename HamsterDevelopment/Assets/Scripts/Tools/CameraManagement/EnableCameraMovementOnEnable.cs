using System;
using MyBox;
using UnityEngine;

public class EnableCameraMovementOnEnable : MonoBehaviour
{
    [SerializeField, MustBeAssigned] private CharacterPivot pivot;

    private void OnEnable()
    {
        pivot.EnableCameraMovement = true;
    }

    private void OnDisable()
    {
        pivot.EnableCameraMovement = false;
    }
}
