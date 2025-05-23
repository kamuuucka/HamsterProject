using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoEventOnTriggerCollision : MonoBehaviour
{
    [SerializeField] private UnityEvent onCollisionEvents;
    [SerializeField] private List<string> collidingTags;
    [SerializeField] private bool onButtonInteraction;
    [SerializeField] private KeyCode buttonToUse;
    [Space(10)]
    [SerializeField] private bool isDebug;

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
                _isColliding = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _isColliding = false;
        SuperDebug.Log("Removing collider!");
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
