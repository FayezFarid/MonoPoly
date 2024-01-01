using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModifierType
{
    PlayerPosition,
    Pay,
    Gain,
}

public enum PlayerTarget
{
    None,
    CurrentTurn,
    AllPlayer,
    Previous,
    //TODO:Custom Target
}

//still need to use the modifiers
[Serializable] //ATTRIBUTE For random effect, use on enable or on seralized wala zibi
public class RandomEffectModifier
{
    //Primary target in case of solo
    public PlayerTarget playerTarget;

    //Secondary target in case for example player pays to all.
    public PlayerTarget SecondaryPlayerTarget;
    public ModifierType modifierType;
    public int value;
}

[CreateAssetMenu(menuName = "Monopoly/ Event  Definition")]
public class EventDefinition : ScriptableObject
{
    public string Description;
    [SerializeField] public List<EventEffectDefinition> EffectModifiers;

    public List<Action> GetActions(GameManager gameManager)
    {
        List<Action> returnList = new List<Action>();
        foreach (var VARIABLE in EffectModifiers)
        {
            returnList.AddRange(VARIABLE.GetActions(gameManager));
            
        }

        return returnList;
    }
}