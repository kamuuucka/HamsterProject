using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class DoEventOnTriggerCollision : MonoBehaviour
{
    [Description("Remember to set the collision to trigger!\n" +
                 "Collision can be on a child of this object.", messageType = MessageType.Warning)] 
    [Tooltip("Events that will happen after the trigger will collide.")]
    [SerializeField] private UnityEvent onCollisionEvents;
    [SerializeField] private List<string> collidingTags = new List<string>(){"Untagged"};
    [SerializeField] private bool onButtonInteraction;
    [SerializeField, HideInInspector] private KeyCode buttonToUse = KeyCode.E;
    [Space(10)]
    [SerializeField, HideInInspector] private bool isDebug;

    private bool _isColliding;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity = false;
        
        if (onButtonInteraction)
        {
            if (isDebug) SuperDebug.Log($"Button to interact: {buttonToUse}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var tag in collidingTags)
        {
            if (other.CompareTag(tag))
            {
                if (isDebug) SuperDebug.Log($"Colliding with {other.gameObject.name}");
                _isColliding = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _isColliding = false;
        if (isDebug) SuperDebug.Log("Does not collide anymore.");
    }

    private void Update()
    {
        if (_isColliding)
        {
            if (onButtonInteraction)
            {
                if (Input.GetKeyUp(buttonToUse))
                {
                    onCollisionEvents?.Invoke();
                }
            }
            else
            {
                onCollisionEvents?.Invoke();
            }
        }
    }
}
