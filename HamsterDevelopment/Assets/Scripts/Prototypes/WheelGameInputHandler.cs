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

    [SerializeField] private WheelRotater wheelRotater;

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
    /// <summary>
    /// Starts the minigame by setting the parameters for the animator
    /// </summary>
    public void StartMinigame()
    {
        if (isFinished) return;
        animator.SetBool("Start", true);
        animator.SetBool("Restart", false);
        wheelRotater.StartSpinning();
    }
    
    /// <summary>
    /// These will be the same for all 3 methods but change depending on input. If you started reading and did this input: Score up. Otherwise, you only speed down.
    /// </summary>
    /// <param name="obj"></param>
    private void OnRightPerformed(InputAction.CallbackContext obj)
    {
        if (readingRight)
        {
            Debug.Log("Right peformed");
            StopReadRightInput();
            score++;
            wheelRotater.SpeedUp();
        }
        else
        {
            wheelRotater.SpeedDown();
        }
    }

    private void OnMidPeformed(InputAction.CallbackContext obj)
    {
        if (readingMid)
        {
            Debug.Log("Mid peformed");
            StopReadMiddleInput();
            score++;
            wheelRotater.SpeedUp();
        }else
        {
            wheelRotater.SpeedDown();
        }
    }

    private void OnLeftperformed(InputAction.CallbackContext obj)
    {
        if (readingLeft)
        {
            Debug.Log("Left peformed");
            StopReadLeftInput();
            score++;
            wheelRotater.SpeedUp();
        }else
        {
            wheelRotater.SpeedDown();
        }
    }

    /// <summary>
    /// These methods get called by the animator to simply start or not start reading
    /// </summary>
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
    
    /// <summary>
    /// Compare the score and determine if you win. If so, call the pass event otherwise reset the minigame and call the onFail event.
    /// </summary>
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
            wheelRotater.Stop();
        }
    }
}
