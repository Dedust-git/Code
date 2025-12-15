using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Cinemachine;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [Header("基础震动")]
    public Vector3 defualtVelocity = new Vector3(0.1f,0.1f,0);
    private CinemachineConfiner2D confinder2D;
    public PlayerEntersBoundEventSO PlayerEnterBoundEvent;
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraShakeEvent;
    public FloatEventSO cameraShakeWithForceEvent;//带力的参数的震动
    public void Awake()
    {
        confinder2D = GetComponent<CinemachineConfiner2D>();
        impulseSource.m_DefaultVelocity = defualtVelocity;
    }

    private void Start()
    {
       // GetNewCameraBounds();
    }
    private void OnEnable()
    {
        PlayerEnterBoundEvent.OnEventRised.AddListener(PlayerEntersBound);
        cameraShakeEvent.OnEventRised.AddListener(OnCameraShakeEvent);
        cameraShakeWithForceEvent.OnEventRised.AddListener(OnCameraShakeWithForceEvent);
    }


    private void OnDisable()
    {
        PlayerEnterBoundEvent.OnEventRised.RemoveListener(PlayerEntersBound);
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

    private void GetNewCameraBounds()
    {
        var obj = GameObject.FindGameObjectsWithTag("Bounds");
        if(obj==null) return;
        confinder2D.m_BoundingShape2D = obj[0].GetComponent<Collider2D>();

        confinder2D.InvalidateCache();
    }
    private void PlayerEntersBound(Collider2D bound)
    {
        confinder2D.m_BoundingShape2D = bound;

        confinder2D.InvalidateCache();
    }
}
