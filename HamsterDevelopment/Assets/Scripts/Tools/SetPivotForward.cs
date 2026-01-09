using System;
using MyBox;
using UnityEngine;

public class SetPivotForward : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private bool useSelfTransform = true;
    [SerializeField, ConditionalField(nameof(useSelfTransform), true)] private Vector3 forward;

    public bool SetPivot = true;

    public void Activate()
    {
    }
    
    private void Update()
    {
        if (SetPivot)
        {
            target.forward = useSelfTransform ? target.position : forward;
        }
    }
}
