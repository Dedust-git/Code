using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/ZoneEventSO")]
public class ZoneEventSO : ScriptableObject
{
    public UnityEvent<Zone> OnEventRised;
    public void RaiseEvent(Zone bound)
    {
        OnEventRised?.Invoke(bound);
    }
}
