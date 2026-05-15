using System;
using MyBox;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CharacterPivot : MonoBehaviour
{
    
    [Header("Settings")]
    [SerializeField] private float _sensitivity = 10;
    [SerializeField] private bool _invertYAxis = false;
    [SerializeField] private bool _invertXAxis = false;
    [SerializeField] private float _lowestAngle = -30;
    [SerializeField] private float _highestAngle = 60;
    [SerializeField] private bool _hideAndLockCursor = true;
    private Vector2 _smoothDelta;
    [SerializeField] private float _smoothSpeed = 12f;
    
    [Header("Input")] 
    [SerializeField] private bool _useActionAsset = true;
    [SerializeField, ConditionalField(nameof(_useActionAsset))] private InputActionAsset _inputActionAsset;
    [SerializeField, ConditionalField(nameof(_useActionAsset), true)] private InputAction _inputAction;
    [FormerlySerializedAs("pivotTransform")]
    [Header("References")]
    [SerializeField] private Transform _pivotTransform;
    public Transform PivotTransform => _pivotTransform;
    private float _pitch = 0f;
    private float _yaw = 0f;
    private float _roll = 0;
    [Header("Debug")]
    [ReadOnly]
    public Vector3 PivotForward
    {
        get
        {
            Vector3 euler = _pivotTransform.rotation.eulerAngles;
            euler.x = 0;
            var result = Quaternion.Euler(euler) * Vector3.forward;
            return result;
        }
    }
    [ReadOnly]
    public Vector3 PivotRight
    {
        get
        {
            Vector3 euler = _pivotTransform.rotation.eulerAngles;
            euler.x = 0;
            var result = Quaternion.Euler(euler) * Vector3.right;
            return result;
        }
    }

    [ReadOnly] public bool EnableCameraMovement = true;

    public void SetEnableCameraMovement(bool enable)
    {
        EnableCameraMovement = enable;
    }
    
    private void OnEnable()
    {
        if (_hideAndLockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if(!_useActionAsset) _inputAction.Enable();
    }

    private void OnDisable()
    {
        if(!_useActionAsset) _inputAction.Disable();
    }

    private void LateUpdate()
    {
        if(!EnableCameraMovement) return;
        if ((!_useActionAsset && _inputAction.enabled) || (_useActionAsset && _inputActionAsset.enabled ))
        {
            Vector2 delta = _useActionAsset
                ? _inputActionAsset.FindAction("Look", true).ReadValue<Vector2>()
                : _inputAction.ReadValue<Vector2>(); 
            
            _smoothDelta = Vector2.Lerp(_smoothDelta, delta, Time.deltaTime * _smoothSpeed);
            delta = _smoothDelta * _sensitivity;
            
            _yaw += _invertXAxis ? delta.x : -delta.x;
            _yaw = Mathf.Repeat(_yaw, 360f);
            
            _pitch += _invertYAxis ? delta.y : -delta.y;
            
            _pitch = Mathf.Clamp(_pitch, _lowestAngle, _highestAngle);
            _pivotTransform.localRotation = Quaternion.Euler(_pitch, _yaw, _roll); 
        }
    }

    private void OnDrawGizmos()
    {
       Gizmos.color = Color.aquamarine;
       Gizmos.DrawRay(this.transform.position, PivotForward);
       
    }

    public void SetPivotForward(Vector3 forward)
    {
        _pivotTransform.LookAt(_pivotTransform.position + forward);
        _smoothDelta = Vector2.zero;
        _pitch = 0f;
        _yaw = 0f;
        _roll = 0f;
        
    }
}
