using System;
using MyBox;
using UnityEngine;

public class SetPivotForward : MonoBehaviour
{
  //  [SerializeField] private Transform target;
    [SerializeField] private CharacterPivot pivot;
    [SerializeField] private bool useSelfTransform = true;
    [SerializeField, ConditionalField(nameof(useSelfTransform), true)] private Vector3 forward;

    public bool SetPivot = true;
    
    [SerializeField] private bool activateOnEnable = true;

    private void OnEnable()
    {
        if (activateOnEnable)
        {
            pivot.EnableCameraMovement =false;
        }
    }
    
    public void Activate()
    {
    }
    
    private void Update()
    {
        if (SetPivot)
        {
            pivot.PivotTransform.forward = useSelfTransform ? pivot.PivotTransform.position : forward;
        }
    }
}
