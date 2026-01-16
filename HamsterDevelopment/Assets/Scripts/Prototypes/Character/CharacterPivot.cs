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
    
    [Header("Input")] 
    [SerializeField] private bool _useActionAsset = true;
    [SerializeField, ConditionalField(nameof(_useActionAsset))] private InputActionAsset _inputActionAsset;
    [SerializeField, ConditionalField(nameof(_useActionAsset), true)] private InputAction _inputAction;
    [FormerlySerializedAs("pivotTransform")]
    [Header("References")]
    [SerializeField] private Transform _pivotTransform;
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

    [ReadOnly] public bool EnableCameraMovement = true;
    
    private void OnEnable()
    {
        
        if(!_useActionAsset) _inputAction.Enable();
    }

    private void OnDisable()
    {
        if(!_useActionAsset) _inputAction.Disable();
    }

    private void Update()
    {
        if (!_useActionAsset && _inputAction.enabled)
        {
            Vector2 delta = _inputAction.ReadValue<Vector2>();
            delta *= Time.deltaTime * _sensitivity;
            _yaw += _invertXAxis ? delta.x : -delta.x;
           
            _pitch += _invertYAxis ? delta.y : -delta.y;
            _pitch = Mathf.Clamp(_pitch, _lowestAngle, _highestAngle);
            _pivotTransform.localRotation = Quaternion.Euler(_pitch, _yaw, _roll);
            
            //pivotTransform.Rotate(pivotTransform.right, _invertYAxis ? delta.y : -delta.y, Space.World);
            //float xAngle = Mathf.Clamp(pivotTransform.rotation.eulerAngles.x, lowestAngle, highestAngle);
            //Debug.Log($"{gameObject.name} Xangle after clamp: {xAngle.ToString("F2")} Rotation Euler: {pivotTransform.rotation.eulerAngles.x.ToString("F2")}. Rotation: {pivotTransform.rotation.x.ToString("F2")}");
            //pivotTransform.rotation = Quaternion.Euler(xAngle, pivotTransform.rotation.eulerAngles.y, 0);
            //pivotTransform.localRotation = Quaternion.Euler(pivotTransform.rotation.eulerAngles.x, pivotTransform.rotation.eulerAngles.y, 0);
        }
    }

    private void OnDrawGizmos()
    {
       Gizmos.color = Color.aquamarine;
       Gizmos.DrawRay(this.transform.position, PivotForward);
       
    }
}
