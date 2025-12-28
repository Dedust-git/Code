using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/StringEventSO")]
public class StringEventSO : ScriptableObject
{
    public UnityEvent<string> OnEventRised;
    public void RaiseEvent(string str)
    {
        OnEventRised?.Invoke(str);
    }
}
