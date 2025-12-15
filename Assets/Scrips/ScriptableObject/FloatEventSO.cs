using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/FloatEventSO")]
public class FloatEventSO : ScriptableObject
{
    public UnityEvent<float> OnEventRised;
    /// <summary>
    /// 传入一个float变量并产生事件
    /// </summary>
    /// <param name="value"></param>
    public void RaiseEvent(float value)
    {
        OnEventRised?.Invoke(value);
    }
}
