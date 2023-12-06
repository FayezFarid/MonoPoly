using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

public class Attributes 
{
   
}

//Can't fill these before runtime. so it can't be used
public class AttributeMonopolyEvent : Attribute
{
    public AttributeMonopolyEvent()
    {
        
    }
}

public static class AttributeHandler
{
    public static  List<Action> FillAction(EventDefinition eventDefinition,GameManager gameManager)
    {
        List<Action> actions = new List<Action>();
        foreach (var modifier in eventDefinition.EffectModifiers)
        {
            if (modifier.modifierType == ModifierType.PlayerPosition)
            {
                actions.Add(CreateActionPlayerPosition(modifier,gameManager));
            }else if (modifier.modifierType == ModifierType.Pay)
            {
                actions.Add(CreateActionPay(modifier, gameManager)) ;
            }else if (modifier.modifierType == ModifierType.Gain)
            {
              actions.Add(CreateActionGain(modifier, gameManager));  
            }
        }

        return actions;
    }
    private static Action CreateActionGain(RandomEffectModifier modifier, GameManager gameManager)
    {
        Action action = null;
        if (modifier.playerTarget == PlayerTarget.AllPlayer)
        {
           
            action = () =>
            {
                foreach (var player in gameManager.TurnManager.AllPlayers)
                {
                    GainMoney(player, modifier.value);
                }    
            };
            return action;
        }
        Player target = GetTargetPlayer(modifier.playerTarget, gameManager);
        action = () => { GainMoney(target, modifier.value);};
        return action;
    }
    private static Action CreateActionPay(RandomEffectModifier modifier, GameManager gameManager)
    {
        Action action = null;
        if (modifier.playerTarget == PlayerTarget.AllPlayer)
        {
           
            action = () =>
            {
                foreach (var player in gameManager.TurnManager.AllPlayers)
                {
                    LoseMoney(player, modifier.value);
                }    
            };
            return action;
        }
        Player target = GetTargetPlayer(modifier.playerTarget, gameManager);
        action = () => { LoseMoney(target, modifier.value);};
        return action;
    }
    private static Action CreateActionPlayerPosition(RandomEffectModifier modifier,GameManager gameManager)
    {
        Action action = null;
        if (modifier.playerTarget == PlayerTarget.AllPlayer)
        {
           
             action = () =>
            {
                foreach (var player in gameManager.TurnManager.AllPlayers)
                {
                    PlacePlayer(player, modifier.value, gameManager);
                }    
            };
             return action;
        }
        Player target = GetTargetPlayer(modifier.playerTarget, gameManager);
        action = () => { PlacePlayer(target, modifier.value, gameManager);};
        return action;
    }
    private static Player GetTargetPlayer(PlayerTarget playerTarget,GameManager gameManager)
    {
        if (playerTarget == PlayerTarget.CurrentTurn)
        {
             return  gameManager.TurnManager.CurrentPlayerTurn;
        }

        if (playerTarget == PlayerTarget.Previous)
        {
            return gameManager.TurnManager.PreviousPlayer;
        }
        SpicyHarissaLogger.LogErrorCritical($"GetTargetPlayer has returned null  Player Target ={playerTarget}");
        return null;
    }
    
    public static void LoseMoney(Player player,int value)
    {
        SpicyHarissaLogger.Log($"Static LoseMoney  [{player}] Value [{value}] ");
        player.ReduceMoney(value);
    }
    public static void GainMoney(Player player, int value)
    {
        SpicyHarissaLogger.Log($"Static GainMoney  [{player}] Value [{value}] ");
        player.IncreaseMoney(value);
    }

    public static void PlacePlayer(Player player, int position,GameManager gameManager)
    {
        SpicyHarissaLogger.Log($"Static PlacePlayer  [{player}] Position [{position}] ");
        gameManager.PlacePlayer(player,position);
    }

}