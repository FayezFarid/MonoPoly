using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monopoly/ Event Effect Definition/GiveStatus Effect")]
public class EventEffectGiveStatus : EventEffectDefinition
{
    [SerializeField] private List<StatusEffectDefinition> _effectDefinitions;
    
    public override List<Action> GetActions(GameManager gameManager,Action onEnd)
    {
        Action action = (() =>
        {
            foreach (var VARIABLE in _effectDefinitions)
            {
                gameManager.CurrentPlayer.ApplyEffectToSelf(VARIABLE);
            }

            onEnd();
        });
        return new List<Action>() { action };
    }
}
