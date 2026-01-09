using System;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public bool Follow = true;
    [SerializeField] private Transform target;
    
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool followRotation;

    private void Update()
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

