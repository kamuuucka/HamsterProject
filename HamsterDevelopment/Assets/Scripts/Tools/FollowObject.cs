using System;
using UnityEngine;

/// <summary>
/// Simple follow script becuase sometimes making an object a child inherits other transforms you dont want to have (rotation).
/// </summary>
public class FollowObject : MonoBehaviour
{
    public bool Follow = true;
    [SerializeField] private Transform target;
    
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool followRotation;

    private void FixedUpdate()
    {
        if (Follow)
        {
            transform.position = target.position + offset;
            if (followRotation)
            {
                transform.rotation = target.rotation;
            }
        }
    }
}

