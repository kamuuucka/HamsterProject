using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WaitSecondsSendEvent : MonoBehaviour
{

    [SerializeField] private float duration;
    [SerializeField] private bool StartOnEnable = false;
    public UnityEvent OnComplete;
    private void OnEnable()
    {
        if (StartOnEnable) Wait();
    }

    public void Wait()
    {
        StartCoroutine(WaitSeconds());
    }
    
    IEnumerator WaitSeconds()
    {
        yield return new WaitForSeconds(duration);
        OnComplete?.Invoke();
    }
}
