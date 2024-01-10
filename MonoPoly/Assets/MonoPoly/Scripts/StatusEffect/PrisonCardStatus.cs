using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monopoly/StatusEffect Definition/Prison Card")]
public class PrisonCardStatus : StatusEffectDefinition
{
    public override EffectType EffectType => EffectType.PrisonPass;

    public override void OnMoneyAdd(StatusEffectInstance instance, ref int moneyValue)
    {
    }

    public override void OnMoneyRemoved(StatusEffectInstance instance, ref int moneyValue)
    {
        
    }

    public override void OnPlayerTurnStarted(StatusEffectInstance instance)
    {
    }

    public override void OnEffectEnded(StatusEffectInstance instance)
    {
    }

    public override void OnPayToPlayer(StatusEffectInstance instance, Player playerPaying, Player playerReceiving, ref int amount)
    {
        throw new System.NotImplementedException();
    }

    public override void OnTurnPass(StatusEffectInstance instance)
    {
    }
}