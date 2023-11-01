using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    float shakerTimer;

    public void Shake(CinemachineVirtualCamera camera, float intensity,float frequency, float time)
    {
        StartCoroutine(ShakeCamera(camera, intensity, frequency, time));
    }
    IEnumerator ShakeCamera(CinemachineVirtualCamera camera, float intensity,float frequency, float time)
    {
        CinemachineBasicMultiChannelPerlin p = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        p.m_AmplitudeGain = intensity;
        p.m_FrequencyGain = frequency;

        yield return new WaitForSeconds(time);

        p.m_AmplitudeGain = 0;
        p.m_FrequencyGain = 0;
    }
}
