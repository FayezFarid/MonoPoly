using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Monopoly/ Event Effect Definition/Player Target")]
public  class EventEffectPlayer : EventEffectDefinition
{
    [SerializeField] public List<RandomEffectModifier> EffectModifiers;

    public override List<Action> GetActions(GameManager gameManager)
    {
        return ExtensionFillEventEffects.FillAction(this, gameManager);
    }
}