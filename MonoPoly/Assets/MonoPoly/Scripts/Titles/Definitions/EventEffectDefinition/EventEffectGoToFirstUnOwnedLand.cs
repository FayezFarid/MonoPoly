using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monopoly/ Event Effect Definition/Go first unowned land")]
public class EventEffectGoToFirstUnOwnedLand : EventEffectDefinition
{
    [SerializeField] private bool payExtraRent;
    private float extraPercentage;

    public override List<Action> GetActions(GameManager gameManager, Action onEnd)
    {
        Action action = GoFirstUnOwnedLand(gameManager, onEnd);
        return new List<Action>() { action };
    }

    private Action GoFirstUnOwnedLand(GameManager gameManager, Action onEnd)
    {
        int playerPosition = gameManager.CurrentPlayer.CurrentPosition;
        LandTitleInstance landTitleInstance = gameManager.TileManager.GetFirstNotOwnedLand(playerPosition);
        if (landTitleInstance == null)
        {
            if (!payExtraRent)
                return null;
            landTitleInstance = gameManager.TileManager.GoToFirstLandFromPosition(playerPosition);
        }

        RandomEffectModifier modifier = new RandomEffectModifier();
        modifier.modifierType = ModifierType.PlayerPosition;
        modifier.value = landTitleInstance.TileLand.PositionInMap;
        modifier.playerTarget = PlayerTarget.CurrentTurn;
        return ExtensionFillEventEffects.CreateActionPlayerPosition(modifier, gameManager, onEnd);
    }

  
}