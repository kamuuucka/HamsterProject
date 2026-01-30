using System;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShot : MonoBehaviour
{
    [SerializeField] private Vector3 _direction;
    [SerializeField] private float _force;
    [SerializeField] private InputActionAsset _actionAsset;
    private HamsterBallMovement _movement;
    [SerializeField, ReadOnly] private bool playerIsInTrigger = false;

    private void OnEnable()
    {
        _actionAsset.FindAction("Interact", true).started += HandleInput;
    }

    private void HandleInput(InputAction.CallbackContext obj)
    {
        if (playerIsInTrigger)
        {
            Fire(_movement);
        }
    }

    public void Fire(HamsterBallMovement ball)
    {
        ball.SetVelocity(_direction.normalized * _force);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        _movement = other.GetComponent<HamsterBallMovement>();
        if (_movement == null)
        {
            playerIsInTrigger = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_movement != null)
        {
            playerIsInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == _movement.gameObject)
        {
            _movement = null;
            playerIsInTrigger = false;
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + _direction * _force, Color.red);    
    }
}
