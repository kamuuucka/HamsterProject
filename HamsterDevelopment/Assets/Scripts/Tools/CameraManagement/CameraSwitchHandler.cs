using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitchHandler : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCameraBase thirdPersonCamera;
    [SerializeField] private CinemachineVirtualCameraBase attentionCamera;

    private CinemachineVirtualCameraBase currentActiveCamera;
    public void GrabAttention(Transform target)
    {
       attentionCamera.Follow = target;
       attentionCamera.gameObject.SetActive(true);
       currentActiveCamera.gameObject.SetActive(false);
    }
    
    public void GrabAttention(Transform target, CinemachineVirtualCameraBase camera)
    {
        camera.Follow = target;
        camera.gameObject.SetActive(true);
        currentActiveCamera.gameObject.SetActive(false);
    }
    public void SwitchCamera(CinemachineVirtualCameraBase camera)
    {
        if(currentActiveCamera == camera) return;
        camera.gameObject.SetActive(true);
        if (currentActiveCamera != null)
        {
            currentActiveCamera.gameObject.SetActive(false);
        }
        currentActiveCamera = camera;
    }

    public void ReleaseAttention()
    {
        attentionCamera.gameObject.SetActive(false);
        currentActiveCamera.gameObject.SetActive(true);
    }
    public void ReleaseAttention(CinemachineVirtualCameraBase camera)
    {
        camera.gameObject.SetActive(false);
        currentActiveCamera.gameObject.SetActive(true);
    }
}
