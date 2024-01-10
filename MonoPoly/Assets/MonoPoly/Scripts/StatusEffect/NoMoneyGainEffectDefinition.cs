using UnityEngine;


[CreateAssetMenu(menuName = "Monopoly/StatusEffect Definition/No Money ")]
public class NoMoneyGainEffectDefinition : StatusEffectDefinition
{
    public override EffectType EffectType => EffectType.NoMoney;


    public override void OnMoneyAdd(StatusEffectInstance instance, ref int moneyValue)
    {
        moneyValue = 0;
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
        
    }
}