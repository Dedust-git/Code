using System;
using Cinemachine;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [Header("基础震动")]
    public Vector3 defualtVelocity = new Vector3(0.1f,0.1f,0);
    private CinemachineConfiner2D confinder2D;
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraShakeEvent;
    public FloatEventSO cameraShakeWithForceEvent;//带力的参数的震动
    public ZoneEventSO PlayerEnterZoneEvent;

    [SerializeField] Zone nowZone;
    private void OnEnable()
    {
        confinder2D = GetComponent<CinemachineConfiner2D>();
        PlayerEnterZoneEvent.OnEventRised.AddListener(OnPlayerEnterZone);
        cameraShakeEvent.OnEventRised.AddListener(OnCameraShakeEvent);
        cameraShakeWithForceEvent.OnEventRised.AddListener(OnCameraShakeWithForceEvent);
    }


    private void OnDisable()
    {
        PlayerEnterZoneEvent.OnEventRised.RemoveListener(OnPlayerEnterZone);
        cameraShakeEvent.OnEventRised.RemoveListener(OnCameraShakeEvent);
        cameraShakeWithForceEvent.OnEventRised.RemoveListener(OnCameraShakeWithForceEvent);
    
    }
    private void OnCameraShakeEvent()
    {
        impulseSource.GenerateImpulse();
    }

    private void OnCameraShakeWithForceEvent(float force)
    {
        impulseSource.m_DefaultVelocity = new Vector3(force,force,0);
        impulseSource.GenerateImpulse();
        impulseSource.m_DefaultVelocity = defualtVelocity;//设置为初始值
    }


    private void OnPlayerEnterZone(Zone zone)
    {
        confinder2D.m_BoundingShape2D = zone.bound;
        nowZone = zone;

        confinder2D.InvalidateCache();
    }
}
