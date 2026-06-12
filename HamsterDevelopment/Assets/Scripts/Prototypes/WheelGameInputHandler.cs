using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WheelGameInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;
    
    private bool readingLeft = false;
    private bool readingRight = false;
    private bool readingMid = false;
    
    private void OnEnable()
    {
        if (!inputActionAsset.enabled)
            inputActionAsset.Enable();

        inputActionAsset.FindAction("RythmLeft").performed += OnLeftperformed;
        inputActionAsset.FindAction("RythmMid").performed += OnMidPeformed;
        inputActionAsset.FindAction("RythmRight").performed += OnRightPerformed;
    }

    private void OnRightPerformed(InputAction.CallbackContext obj)
    {
        if (readingRight)
        {
            Debug.Log("Right peformed");
            StopReadRightInput();
        }
    }

    private void OnMidPeformed(InputAction.CallbackContext obj)
    {
        if (readingMid)
        {
            Debug.Log("Mid peformed");
            StopReadMiddleInput();
        }
    }

    private void OnLeftperformed(InputAction.CallbackContext obj)
    {
        if (readingLeft)
        {
            Debug.Log("Left peformed");
            StopReadLeftInput();
        }
    }


    public void StartReadLeftInput()
    {
        readingLeft = true;
    }

    public void StopReadLeftInput()
    {
        readingLeft = false;
    }

    public void StartReadRightInput()
    {
        readingRight =  true;
    }
    public void StopReadRightInput()
    {
        readingRight = false;
    }
    public void StartReadMiddleInput()
    {
        readingMid = true;
    }
    public void StopReadMiddleInput()
    {
        readingMid = false;
    }
}
