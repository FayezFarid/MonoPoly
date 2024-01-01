using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Monopoly/ Event Effect Definition/Prison")]
public class EventEffectPrison : EventEffectDefinition
{
    [SerializeField] private PrisonStatusEffectDefinition prisonStatusEffectDefinition;

    public override List<Action> GetActions(GameManager gameManager)
    {
        Action action = () => { gameManager.CurrentPlayerGoesPrison(prisonStatusEffectDefinition); };
        return new List<Action>() { action };
    }
}