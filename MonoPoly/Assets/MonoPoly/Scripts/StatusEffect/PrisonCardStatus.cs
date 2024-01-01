using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monopoly/StatusEffect Definition/Prison Card")]
public class PrisonCardStatus : StatusEffectDefinition
{
    public override EffectType EffectType => EffectType.PrisonPass;

    public override void OnTurnPass(StatusEffectInstance instance)
    {
       
    }
}
