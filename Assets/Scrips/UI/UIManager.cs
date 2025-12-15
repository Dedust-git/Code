using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class UIManager : MonoBehaviour
{
    public PlayerStateBar psb;
    [Header("事件监听")]
    public CharacterEventSO characterEvent;
    private void OnEnable()
    {
        characterEvent.OnEventRised.AddListener(OnCharaterEvent);
    }

    private void OnDisable()
    {
        characterEvent.OnEventRised.RemoveListener(OnCharaterEvent);
    }

    private void OnCharaterEvent(Characters character)
    {
        float Healthpercentage = (float)character.CurrentHealth / (float)character.MaxHealth;
        float Powerpercentage = (float)character.CurrentPower/(float)character.MaxPower;
        psb.OnPowerChanged(Powerpercentage);
        psb.OnHealthChanged(Healthpercentage);
    }
}
