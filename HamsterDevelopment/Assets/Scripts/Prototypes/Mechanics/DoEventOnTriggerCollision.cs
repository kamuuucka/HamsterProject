using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class DoEventOnTriggerCollision : MonoBehaviour
{
    [Description("Remember that this object you want to collide with needs to have the collision set to trigger!\n" +
                 "Collision can be on a child of the object.", messageType = MessageType.Warning)] 
    [SerializeField] private UnityEvent onCollisionEvents;
    [SerializeField] private List<string> collidingTags;
    [SerializeField] private bool onButtonInteraction;
    [SerializeField, HideInInspector] private KeyCode buttonToUse = KeyCode.E;
    [Space(10)]
    [SerializeField, HideInInspector] private bool isDebug;

    private bool _isColliding;

    private void Awake()
    {
        if (onButtonInteraction)
        {
            SuperDebug.Log($"{buttonToUse}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var tag in collidingTags)
        {
            if (other.CompareTag(tag))
            {
                if (isDebug) SuperDebug.Log("Colliding");
                _isColliding = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _isColliding = false;
        if (isDebug) SuperDebug.Log("Removing collider!");
    }

    private void Update()
    {
        if (_isColliding)
        {
            if (onButtonInteraction)
            {
                if (Input.GetKeyUp(buttonToUse))
                {
                    if (isDebug) SuperDebug.Log("Triggered!");
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
