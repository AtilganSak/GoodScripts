using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public enum CameraType
{
    Idle,
    Gameplay,
    FPS,
    Giant
}
public class CinemachineCameraManager : MonoBehaviourSingletonPersistent<CinemachineCameraManager>
{
    [System.Serializable]
    public struct CameraData
    {
        public CameraType type;
        public CinemachineVirtualCamera camera;
    }

    [SerializeField] CameraType startCamera;
    [SerializeField] CameraData[] cameras;

    CameraData currentCamera;

    float transitionTime;
    bool startedShake;

    Action cameraBlendCallback;

    private void Start()
    {
        SwitchCamera(startCamera);

        transitionTime = CinemachineCore.Instance.FindPotentialTargetBrain(currentCamera.camera).m_DefaultBlend.m_Time;
    }
    public void SwitchCamera(CameraType camera)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].type == camera)
            {
                int prevPriorty = 1;
                if (currentCamera.camera != null)
                {
                    prevPriorty = currentCamera.camera.Priority;
                    currentCamera.camera.Priority = cameras[i].camera.Priority;
                }
                currentCamera = cameras[i];
                currentCamera.camera.Priority = prevPriorty;
            }
        }
    }
    public void SwitchCamera(CameraType camera, Action callback)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].type == camera)
            {
                int prevPriorty = 1;
                if (currentCamera.camera != null)
                {
                    prevPriorty = currentCamera.camera.Priority;
                    currentCamera.camera.Priority = cameras[i].camera.Priority;
                }
                currentCamera = cameras[i];
                currentCamera.camera.Priority = prevPriorty;
            }
        }
        cameraBlendCallback = callback;
        Invoke("CameraBlended", transitionTime);
    }
    public void SwitchCamera(CinemachineVirtualCamera camera, Action callback)
    {
        int prevPriorty = 1;
        if (currentCamera.camera != null)
        {
            prevPriorty = currentCamera.camera.Priority;
            currentCamera.camera.Priority = camera.Priority;
        }
        currentCamera.camera = camera;
        currentCamera.camera.Priority = prevPriorty;
        cameraBlendCallback = callback;
        Invoke("CameraBlended", transitionTime);
    }
    public void SetCamera(CameraType cameraType, CinemachineVirtualCamera camera)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].type == cameraType)
            {
                cameras[i].camera = camera;
            }
        }
    }
    public void SetTarget(Transform tr, CameraType cameraType)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].type == cameraType)
            {
                cameras[i].camera.m_Follow = tr;
                cameras[i].camera.m_LookAt = tr;
            }
        }
    }
    private void CameraBlended()
    {
        if (cameraBlendCallback != null)
        {
            cameraBlendCallback.Invoke();
            cameraBlendCallback = null;
        }
    }
    public CinemachineVirtualCamera GetCamera(CameraType cameraType)
    {
        if (cameras != null)
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i].type == cameraType)
                {
                    return cameras[i].camera;
                }
            }
        }
        return null;
    }
    public void Shake(float intensity, float frequency, float time)
    {
        StartCoroutine(ShakeCamera(intensity, frequency, time));
    }
    public void StartShake(float intensity, float frequency)
    {
        if (!startedShake)
        {
            startedShake = true;

            CinemachineBasicMultiChannelPerlin p = currentCamera.camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            p.m_AmplitudeGain = intensity;
            p.m_FrequencyGain = frequency;
        }
    }
    public void StopShake()
    {
        if (startedShake)
        {
            startedShake = false;
            CinemachineBasicMultiChannelPerlin p = currentCamera.camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            p.m_AmplitudeGain = 0;
            p.m_FrequencyGain = 0;
        }
    }
    IEnumerator ShakeCamera(float intensity, float frequency, float time)
    {
        CinemachineBasicMultiChannelPerlin p = currentCamera.camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        p.m_AmplitudeGain = intensity;
        p.m_FrequencyGain = frequency;

        yield return new WaitForSeconds(time);

        p.m_AmplitudeGain = 0;
        p.m_FrequencyGain = 0;
    }
}
