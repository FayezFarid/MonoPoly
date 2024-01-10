using UnityEngine;


[CreateAssetMenu(menuName = "Monopoly/StatusEffect Definition/Extra Money ")]
public class ExtraMoneyStatusEffect : StatusEffectDefinition
{
    [SerializeField, Range(0, 1)] private float percentageToAdd;
    public override EffectType EffectType => EffectType.ExtraMoney;

    public override void OnMoneyAdd(StatusEffectInstance instance, ref int moneyValue)
    {
        int Percent = Mathf.FloorToInt(moneyValue * percentageToAdd);
        moneyValue += Percent;
    }
    public override void OnPayToPlayer(StatusEffectInstance instance, Player playerPaying, Player playerReceiving, ref int amount)
    {
        int Percent = Mathf.FloorToInt(amount * percentageToAdd);
        amount += Percent;
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

   
}