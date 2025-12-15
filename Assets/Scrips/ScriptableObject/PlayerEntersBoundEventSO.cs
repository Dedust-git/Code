using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/PlayerEntersBoundEventSO")]
public class PlayerEntersBoundEventSO : ScriptableObject
{
    public UnityEvent<Collider2D> OnEventRised;
    public void RaiseEvent(Collider2D bound)
    {
        OnEventRised?.Invoke(bound);
    }
}
