using System;
using UnityEngine;

public class WheelMover : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 2;
    
    [SerializeField] private float spinSpeed;

    private float speedUpCount;
    private bool spinning = false;
    private void OnEnable()
    {
        speedUpCount = 0;
    }

    private void FixedUpdate()
    {
        if(spinning)
            transform.Rotate(transform.forward, baseSpeed + spinSpeed * speedUpCount);
    }

    public void StartSpinning()
    {
        spinning = true;
    }
    
    public void SpeedUp()
    {
        speedUpCount++;
    }

    public void SpeedDown()
    {
        speedUpCount--;
        if (speedUpCount <= 0)
        {
            speedUpCount = 0;
        }
    }
    
    public void Stop()
    {
        speedUpCount = 0;
        spinning = false;
    }
}
