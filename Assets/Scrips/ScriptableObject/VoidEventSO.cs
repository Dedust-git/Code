using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/VoidEventSO")]
public class VoidEventSO : ScriptableObject
{
    public UnityEvent OnEventRised;
    public void RaiseEvent()
    {
        OnEventRised?.Invoke();
    }
}
