using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WheelGameInputHandler : MonoBehaviour
{
    private bool isFinished = false;
    [SerializeField] private InputActionAsset inputActionAsset;
    
    private bool readingLeft = false;
    private bool readingRight = false;
    private bool readingMid = false;

    [SerializeField] private float baseSpeed = 2;
    
    [SerializeField] private Transform wheelRoot;
    [SerializeField] private float spinSpeed;

    [SerializeField] private int scoreToBeat = 2;
    private Animator animator;
    private int score = 0;

    public UnityEvent OnPassed;
    public UnityEvent OnFailed;
    private void OnEnable()
    {
        if (!inputActionAsset.enabled)
            inputActionAsset.Enable();

        inputActionAsset.FindAction("RythmLeft").performed += OnLeftperformed;
        inputActionAsset.FindAction("RythmMid").performed += OnMidPeformed;
        inputActionAsset.FindAction("RythmRight").performed += OnRightPerformed;
        score = 0;
        animator = GetComponent<Animator>();
        
    }

    public void StartMinigame()
    {
        if (isFinished) return;
        animator.SetBool("Start", true);
        animator.SetBool("Restart", false);
    }
    
    private void OnRightPerformed(InputAction.CallbackContext obj)
    {
        if (readingRight)
        {
            Debug.Log("Right peformed");
            StopReadRightInput();
            score++;
        }
    }

    private void OnMidPeformed(InputAction.CallbackContext obj)
    {
        if (readingMid)
        {
            Debug.Log("Mid peformed");
            StopReadMiddleInput();
            score++;
        }
    }

    private void OnLeftperformed(InputAction.CallbackContext obj)
    {
        if (readingLeft)
        {
            Debug.Log("Left peformed");
            StopReadLeftInput();
            score++;
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

    private void FixedUpdate()
    {
        wheelRoot.Rotate(transform.forward, baseSpeed + spinSpeed * score);
    }

    public void EndMinigame()
    {
        Debug.Log("End Minigame. Score: "  + score);
        if (score >= scoreToBeat)
        {
            isFinished = true;
            Debug.Log("Success!");
            OnPassed?.Invoke();
            gameObject.SetActive(false);
        }
        else
        {
            
            Debug.Log("Fail!");
            OnFailed?.Invoke();
            animator.SetBool("Start", false);
            animator.SetBool("Restart", true);
            gameObject.SetActive(false);
        }
    }
}
