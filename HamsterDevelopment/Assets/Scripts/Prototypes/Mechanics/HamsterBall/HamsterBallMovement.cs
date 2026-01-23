using System;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class HamsterBallMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _turnSpeed = 10;
    [SerializeField] private float _turnThreshold = -0.2f;

    private Rigidbody _rigidbody;
    private CharacterPivot _characterPivot;
    [Header("Input")] 
    private bool _useRegularForward = false;
    [SerializeField] private bool _useActionAsset = true;
    [SerializeField, ConditionalField(nameof(_useActionAsset))] private InputActionAsset _inputActionAsset;
    [SerializeField, ConditionalField(nameof(_useActionAsset), true)] private InputAction _inputAction;


    public Vector3 CurrentVelocity
    {
        get
        {
            if(_rigidbody == null) return Vector3.zero;
            else return _rigidbody.linearVelocity;
        }
    }

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _characterPivot = GetComponent<CharacterPivot>();
        _useRegularForward = _characterPivot == null;
        if(!_useActionAsset) _inputAction.Enable();
    }
    
    
    
    private void OnDisable()
    {
        if(!_useActionAsset) _inputAction.Disable();
    }

    private void Update()
    {
        if ((!_useActionAsset && _inputAction.enabled) || (_useActionAsset && _inputActionAsset.enabled))
        {
           Vector2 input = _useActionAsset ? _inputActionAsset.FindAction("Move", true).ReadValue<Vector2>()
               : _inputAction.ReadValue<Vector2>();
           var forwardDirection = new Vector3();
           var rightDirection = new Vector3();
           
           if (_useRegularForward)
           {
               forwardDirection = transform.forward * input.x;
               rightDirection = transform.right * input.y;
           }
           else
           {
               forwardDirection = _characterPivot.PivotForward * input.y;
               rightDirection = _characterPivot.PivotRight * input.x;
           }
           
           var force = (rightDirection + forwardDirection).normalized * _speed * Time.deltaTime;
           if (Vector3.Dot(_rigidbody.linearVelocity, force) < _turnThreshold)
           {
               force *= _turnSpeed;
           }
           _rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }

    public void AddSpeedBoost(float speed)
    {
        _rigidbody.AddForce(CurrentVelocity * speed, ForceMode.Impulse);
    }
}
