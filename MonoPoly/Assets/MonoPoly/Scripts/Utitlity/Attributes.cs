using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
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

public static class ExtensionFillEventEffects
{
    public static List<Action> FillAction(EventEffectPlayer eventDefinition, GameManager gameManager, Action onEnd)
    {
        List<Action> actions = new List<Action>();
        foreach (var modifier in eventDefinition.EffectModifiers)
        {
            if (modifier.modifierType == ModifierType.PlayerPosition)
            {
                actions.Add(CreateActionPlayerPosition(modifier, gameManager, onEnd));
            }
            else if (modifier.modifierType == ModifierType.Pay)
            {
                actions.Add(CreateActionPay(modifier, gameManager, onEnd));
            }
            else if (modifier.modifierType == ModifierType.Gain)
            {
                actions.Add(CreateActionGain(modifier, gameManager, onEnd));
            }
        }

        return actions;
    }

    private static Action CreateActionGain(RandomEffectModifier modifier, GameManager gameManager, Action onEnd)
    {
        Action action = null;

        action = () =>
        {
            List<Player> target = GetTargetPlayer(modifier.playerTarget, modifier.SecondaryPlayerTarget,
                gameManager, out List<Player> secondaryTargetList);

            if (secondaryTargetList != null)
            {
                foreach (var PlayerSecondary in secondaryTargetList)
                {
                    foreach (var PlayerPrimary in target)
                    {
                        PlayerGivesMoneyTo(PlayerSecondary, PlayerPrimary, modifier.value);
                    }
                }
            }
            else
            {
                foreach (var player in target)
                {
                    GainMoney(player, modifier.value);
                }
            }

            onEnd();
        };
        return action;
    }

    private static Action CreateActionPay(RandomEffectModifier modifier, GameManager gameManager, Action onEnd)
    {
        Action action = null;

        action = () =>
        {
            List<Player> target = GetTargetPlayer(modifier.playerTarget, modifier.SecondaryPlayerTarget,
                gameManager, out List<Player> secondaryTargetList);

            //TODO: should lose money accordingly
            if (secondaryTargetList != null)
            {
                foreach (var PlayerSecondary in secondaryTargetList)
                {
                    foreach (var PlayerPrimary in target)
                    {
                        PlayerGivesMoneyTo(PlayerPrimary, PlayerSecondary, modifier.value);
                    }
                }
            }
            else
            {
                foreach (var player in target)
                {
                    PlayerGivesMoneyTo(player, gameManager.TileManager.ParkingTileInstance, modifier.value);
                }
            }

            onEnd();
        };
        return action;
    }

    public static Action CreateActionPlayerPosition(RandomEffectModifier modifier, GameManager gameManager,
        Action onEnd)
    {
        Action action = null;

        action = () =>
        {
            List<Player> target = GetTargetPlayer(modifier.playerTarget, modifier.SecondaryPlayerTarget,
                gameManager, out List<Player> secondaryTargetList);

            _gameManager = gameManager;
            _onEnd = onEnd;

            gameManager.OnPlayerLandedOnTile += OnPlayerLanded;
            foreach (var player in target)
            {
                PlacePlayer(player, modifier.value, gameManager);
            }
        };
        return action;
    }

    private static GameManager _gameManager;
    private static Action _onEnd;

    private static void OnPlayerLanded(Player player, Tile tile)
    {
        SpicyHarissaLogger.Log($"Player Landed going trigger on End ",LogLevel.Verbose);
        _onEnd();
        _gameManager.OnPlayerLandedOnTile -= OnPlayerLanded;
    }

    private static List<Player> GetSingleTarget(PlayerTarget playerTarget, GameManager gameManager)
    {
        if (playerTarget == PlayerTarget.None)
        {
            return null;
        }

        List<Player> listToReturn = new List<Player>(gameManager.TurnManager.ActivePlayers.Count - 1);
        if (playerTarget == PlayerTarget.CurrentTurn)
        {
            listToReturn.Add(gameManager.TurnManager.CurrentPlayerTurn);
        }
        else if (playerTarget == PlayerTarget.Previous)
        {
            listToReturn.Add(gameManager.TurnManager.PreviousPlayer);
        }
        else if (playerTarget == PlayerTarget.AllPlayer)
        {
            listToReturn.AddRange(gameManager.TurnManager.ActivePlayers);
        }


        return listToReturn;
    }

    private static List<Player> GetTargetPlayer(PlayerTarget playerTarget, PlayerTarget secondaryTarget,
        GameManager gameManager, out List<Player> secondaryTargetList)
    {
        //Odd case
        if (playerTarget == secondaryTarget)
        {
            SpicyHarissaLogger.LogErrorCritical(
                "Ignoring  Player Target because Secondary and Primary and equal and applying to current only ");

            secondaryTargetList = null;
            return new List<Player>(1) { gameManager.CurrentPlayer };
        }

        List<Player> listToReturn = GetSingleTarget(playerTarget, gameManager);
        secondaryTargetList = GetSingleTarget(secondaryTarget, gameManager);

        if (playerTarget == PlayerTarget.AllPlayer)
        {
            listToReturn.Remove(GetSingleTarget(secondaryTarget, gameManager)[0]);
        }

        if (secondaryTarget == PlayerTarget.AllPlayer)
        {
            secondaryTargetList.Remove(GetSingleTarget(playerTarget, gameManager)[0]);
        }

        return listToReturn;
    }

    // public static void PlayerGives
    public static void PlayerGivesMoneyTo(IMoneyTrader player, IMoneyTrader playerToGiveMoneyTo, int value)
    {
        SpicyHarissaLogger.Log($"Static PayPlayer [{player}] Gain [{playerToGiveMoneyTo}] Value [{value}] ",
            LogLevel.Verbose);
        player.PayPlayer(playerToGiveMoneyTo, value);
    }

    public static void LoseMoney(Player player, int value)
    {
        SpicyHarissaLogger.Log($"Static LoseMoney  [{player}] Value [{value}] ", LogLevel.Verbose);
        player.ReduceMoney(value);
    }

    public static void GainMoney(Player player, int value)
    {
        SpicyHarissaLogger.Log($"Static GainMoney  [{player}] Value [{value}] ", LogLevel.Verbose);
        player.IncreaseMoney(value);
    }

    public static void PlacePlayer(Player player, int position, GameManager gameManager)
    {
        SpicyHarissaLogger.Log($"Static PlacePlayer  [{player}] Position [{position}] ", LogLevel.Verbose);

        gameManager.PlacePlayerCalcuated(player, position, false, 2);
        // gameManager.PlacePlayer(player, position);
    }
}