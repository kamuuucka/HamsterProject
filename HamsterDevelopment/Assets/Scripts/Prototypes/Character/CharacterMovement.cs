using System;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 5f;

    private void Update()
    {
        var move = new Vector3(Input.GetAxis("Horizontal"), 0, -Input.GetAxis("Vertical"));

        characterController.Move(move * Time.deltaTime * speed);
    }
}
