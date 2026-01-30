using System;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class HamsterBallMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _turnSpeed = 10;
    [SerializeField] private float _turnThreshold = -0.2f;
    
    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 10;
    [SerializeField,ReadOnly] private float coyoteTimer = 0;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField, ReadOnly] private bool _isGrounded;
    [SerializeField, ReadOnly] private bool _canJump;

    private Rigidbody _rigidbody;
    private CharacterPivot _characterPivot;
    
    
    
    [Header("Input Settings")] 
    [SerializeField] private bool _useActionAsset = true;
    private bool _useRegularForward = false;
    [SerializeField, ConditionalField(nameof(_useActionAsset))] private InputActionAsset _inputActionAsset;
    [SerializeField, ConditionalField(nameof(_useActionAsset), true)] private InputAction _inputAction;

    [Header("References")]
    [SerializeField] private Transform groundCheckLocation;
    
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
        else
        {
            _inputActionAsset.FindAction("Jump").Enable();
            _inputActionAsset.FindAction("Jump").performed += Jump;
            _inputActionAsset.FindAction("Look").Enable();
        }
            
            
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if(!_canJump) return;
        
        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        _canJump = false;
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

        if (!_isGrounded && _canJump)
        {
            coyoteTimer += Time.deltaTime;
            if (coyoteTimer >= coyoteTime)
            {
                _canJump = false;
            }
        }

    }

    private void FixedUpdate()
    {
        if (Physics.CheckSphere(groundCheckLocation.position, .25f, _groundMask))
        {
            _isGrounded = true;
            _canJump = true;
            coyoteTimer = 0;
        }
        else
        {
            _isGrounded = false;
        }
    }

    public void AddSpeedBoost(float speed)
    {
        _rigidbody.AddForce(CurrentVelocity * speed, ForceMode.Impulse);
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        _rigidbody.linearVelocity = newVelocity;
    }
}
