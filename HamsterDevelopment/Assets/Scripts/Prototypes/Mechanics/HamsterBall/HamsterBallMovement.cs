using System;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class HamsterBallMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public bool AllowMovement = true;
    [SerializeField] private float _speed = 10;
    [SerializeField, Tooltip("The threshold required to apply your turn speed. This is for a dot product see how to use it:\n<b>falstad.com/dotproduct/</b>\nConsider the red line the direction of your ball and the blue line the input.")] 
    private float _turnThreshold = -0.2f;
    [SerializeField, Tooltip("A multiplier of your speed when the turn threshold is reached.")] private float _turnSpeed = 10;
  
    [Header("Jump Settings")]
    [SerializeField, Tooltip("How high you can jump")] private float _jumpForce = 10;
    [SerializeField,ReadOnly] private float coyoteTimer = 0;
    [SerializeField, Tooltip("How long can you be off the ground before youre also no longer allowed to jump.")]
    private float coyoteTime = 0.2f;
    [SerializeField, Tooltip("What is the ground?")] private LayerMask _groundMask;
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
    [SerializeField,Tooltip("A transform you should place at the bottom of the sphere and give it the <b>Follow Object</b> component so it follows without inheriting rotation")] 
    private Transform groundCheckLocation;
    
    public Vector3 CurrentVelocity
    {
        get
        {
            if(_rigidbody == null) return Vector3.zero;
            else return _rigidbody.linearVelocity;
        }
    }

    public void SetPivotToDirection(Vector3 direction)
    {
        _characterPivot.SetPivotForward(direction);
    }
    
    public void HardSetPivot(Vector3 pivot)
    {
        _characterPivot.EnableCameraMovement = false;
        _characterPivot.SetPivotForward(pivot);
    }

    public void ReleaseHardPivot()
    {
        _characterPivot.EnableCameraMovement = true;
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
        if(!_canJump || !AllowMovement) return;
        
        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        _canJump = false;
    }


    private void OnDisable()
    {
        if(!_useActionAsset) _inputAction.Disable();
    }

    private void Update()
    {
        if (!AllowMovement)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            return;
        }
        
        if ((!_useActionAsset && _inputAction.enabled) || (_useActionAsset && _inputActionAsset.enabled))
        {
           //Get your input direction 
           Vector2 input = _useActionAsset ? _inputActionAsset.FindAction("Move", true).ReadValue<Vector2>()
               : _inputAction.ReadValue<Vector2>();
           
           var forwardDirection = new Vector3();
           var rightDirection = new Vector3();
           //If you're using a regular forward take your transforms current forward. If using  character pivot use that forward and right instead.
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
           //Determine force in a framerate independant way
           var force = (rightDirection + forwardDirection).normalized * _speed * Time.deltaTime;
           // If the new force input's direction is lower than the turn threshold apply turn speed.
           // See: https://www.falstad.com/dotproduct/ for more information
           if (Vector3.Dot(_rigidbody.linearVelocity, force) < _turnThreshold)
           {
               force *= _turnSpeed;
           }
           _rigidbody.AddForce(force, ForceMode.Impulse);
        }
        //If our user is off the ground without jumping start the coyote timer
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
        //Check for ground.
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
    
    /// <summary>
    /// Add a force multiplier.
    /// </summary>
    /// <param name="speed"></param>
    public void AddSpeedBoost(float speed)
    {
        _rigidbody.AddForce(CurrentVelocity * speed, ForceMode.Impulse);
    }
    /// <summary>
    /// Set the velocity of your sphere.
    /// </summary>
    /// <param name="newVelocity"></param>
    public void SetVelocity(Vector3 newVelocity)
    {
        _rigidbody.linearVelocity = newVelocity;
    }
}
