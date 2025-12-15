using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    public UnityEvent<Characters> OnEventRised;
    public void RaiseEvent(Characters character)
    {
        OnEventRised?.Invoke(character);
    }
}
