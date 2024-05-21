using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraEffectManager : MonoBehaviour
{
    #region Unity callback methods

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    #endregion // Unity callback methods

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer > 0.0f)
                return;

            _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
        }
    }

    #region Public methods

    public void ShakeCamera(float intensity, float time)
    {
        _cinemachineBasicMultiChannelPerlin =
            _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }

    #endregion // Public methods

    #region Data members

    public static CameraEffectManager Instance { get; private set; }
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;

    private float shakeTimer;

    #endregion // Data members
}