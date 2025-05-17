using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoEventOnTriggerCollision : MonoBehaviour
{
    [SerializeField] private UnityEvent onCollisionEvents;
    [SerializeField] private List<string> collidingTags;
    [Space(10)]
    [SerializeField] private bool isDebug;
    private void OnTriggerEnter(Collider other)
    {
        foreach (var tag in collidingTags)
        {
            if (other.gameObject.CompareTag(tag))
            {
                if (isDebug) SuperDebug.Log("Colliding!");
                onCollisionEvents?.Invoke();
            }
        }
    }
}
