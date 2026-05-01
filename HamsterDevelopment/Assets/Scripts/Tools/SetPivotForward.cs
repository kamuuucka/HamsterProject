using System;
using MyBox;
using UnityEngine;

public class SetPivotForward : MonoBehaviour
{
  //  [SerializeField] private Transform target;
    [SerializeField] private CharacterPivot pivot;
    [SerializeField] private bool useSelfTransform = true;
    [SerializeField, ConditionalField(nameof(useSelfTransform), true)] private Vector3 forward;
    [SerializeField] private bool delayPivot = false;
    [SerializeField,ConditionalField(nameof(delayPivot))] private float delayTime = 1;
    private bool delayed = false;
    private float timer = 0;
    public bool SetPivot = false;
    
    [SerializeField] private bool activateOnEnable = true;
    
    private void OnEnable()
    {
        if (activateOnEnable)
        {
            
            if (delayPivot)
            {
                delayed = true;
            }
            else
            {
                Activate();
            }
        }
    }
    
    public void Activate()
    {
        pivot.EnableCameraMovement = false;
        SetPivot = true;
    }

    
    private void Update()
    {
        if (delayPivot && !SetPivot)
        {
            timer += Time.deltaTime;
            if (timer >= delayTime)
            {
                Activate();
                timer = 0;
            }
        }
        if (SetPivot)
        {
            pivot.PivotTransform.forward = useSelfTransform ? pivot.PivotTransform.position : forward;
        }
    }
}
