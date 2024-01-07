using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monopoly/ Event Effect Definition/RemoveStatus Effect")]
public class EventEffectRemoveStatus : EventEffectDefinition
{
    [SerializeField] private EffectType effectType;

    public override List<Action> GetActions(GameManager gameManager)
    {
        Action action = () => { gameManager.CurrentPlayer.RemoveEffectByType(effectType); };
        return new List<Action>() { action };
    }
}