using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    [ReadOnly, SerializeField] private bool _isFading = false;
    [SerializeField] private float _fadeDuration = 1f;

    private CanvasGroup _canvasGroup;

    private void OnEnable()
    {
        if(_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeToBlack()
    {
        if (_isFading) return;
        _isFading = true;
        _canvasGroup.alpha = 0f;
        StartCoroutine(FadeToValue(1));
    }

    public void FadeToTransparent()
    {
        if (_isFading) return;
        _isFading = true;
        _canvasGroup.alpha = 1f;
        StartCoroutine(FadeToValue(0));
    }
    private IEnumerator FadeToValue(float value)
    {
        float time = 0;
        var startValue = _canvasGroup.alpha;
        
        while (time < _fadeDuration)
        {
            _canvasGroup.alpha = Mathf.Lerp(startValue, value, time / _fadeDuration);

            time += Time.deltaTime;
            yield return null;
        }
        _canvasGroup.alpha = value;
        
        _isFading = false;
    }
    
}
