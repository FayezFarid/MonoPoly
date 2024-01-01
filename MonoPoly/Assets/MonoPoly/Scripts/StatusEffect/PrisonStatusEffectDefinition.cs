using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monopoly/StatusEffect Definition/Prison ")]
public class PrisonStatusEffectDefinition : StatusEffectDefinition
{
    public override EffectType EffectType => EffectType.Prison;
    public static Action<StatusEffectInstance> CurrentPlayerInPrison;
    
    
    public override void OnMoneyAdd(StatusEffectInstance instance,ref int moneyValue)
    {
        moneyValue = 0;
    }
    public override void OnPlayerTurnStarted(StatusEffectInstance instance)
    {
        CurrentPlayerInPrison?.Invoke(instance);
    }
  
}
