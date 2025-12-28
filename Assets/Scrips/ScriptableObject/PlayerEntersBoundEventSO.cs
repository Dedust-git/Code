using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/Collider2DEventSO")]
public class Collider2DEventSO : ScriptableObject
{
    public UnityEvent<Collider2D> OnEventRised;
    public void RaiseEvent(Collider2D bound)
    {
        OnEventRised?.Invoke(bound);
    }
}
