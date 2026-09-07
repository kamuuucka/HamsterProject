using System.Collections;
using MyBox;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class AttentionGrabber : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float duration;
    [SerializeField] private CameraSwitchHandler cameraSwitchHandler;
    [SerializeField] private UnityEvent onSwitch;
    [SerializeField] private bool hardCut = false;
    [SerializeField, ConditionalField(nameof(hardCut))]
    private CinemachineVirtualCameraBase cameraToCutTo;
    
    [SerializeField] private bool setFOV = false;
    [SerializeField, ConditionalField(nameof(setFOV))]
    private float cameraFOV = 30;
    
    [ButtonMethod]
    public void GrabAttention()
    {
        StartCoroutine(StartSwitch());
    }

    private IEnumerator StartSwitch()
    {
        
        if (hardCut)
        {
            cameraSwitchHandler.GrabAttention(target, cameraToCutTo);
        }
        else
        {
            if(setFOV)
                cameraSwitchHandler.GrabAttention(target, cameraFOV);
                else
                cameraSwitchHandler.GrabAttention(target);
        }
       
        yield return new WaitForSeconds(0.3f);
        onSwitch?.Invoke();
        yield return new WaitForSeconds(duration);
        if (hardCut)
        {
            cameraSwitchHandler.ReleaseAttention(cameraToCutTo);
        }
        else
        {
            cameraSwitchHandler.ReleaseAttention();
        }
        
    }
    
}
