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
[Serializable]
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
    [SerializeField] protected List<EventEffectDefinition> EffectModifiers;

    public virtual List<Action> GetActions(GameManager gameManager)
    {
        List<Action> returnList = new List<Action>();
        foreach (var VARIABLE in EffectModifiers)
        {
            List<Action> actions = VARIABLE.GetActions(gameManager);
            if (actions != null)
                returnList.AddRange(actions);
        }

        return returnList;
    }
}