using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Monopoly/ Event Effect Definition/Give land")]
public class EventEffectGiveLand : EventEffectDefinition
{
    [SerializeField] private int preciseLandToGive;
    [SerializeField] private bool searchFromCurrentPosition;

    public override List<Action> GetActions(GameManager gameManager,Action onEnd)
    {
        LandTitleInstance landTitleInstance;
        if (preciseLandToGive == 0)
        {
            if (searchFromCurrentPosition)
            {
                landTitleInstance =
                    gameManager.TileManager.GetFirstNotOwnedLand(gameManager.CurrentPlayer.CurrentPosition);
            }
            else
            {
                landTitleInstance = gameManager.TileManager.GetFirstNotOwnedLand();
            }
        }
        else
        {
            Tile tile = gameManager.TileManager[preciseLandToGive];
            if (tile.TitleType is not TitleType.Land or TitleType.Station)
            {
                SpicyHarissaLogger.LogErrorCritical($"Tile at Index [{preciseLandToGive}] is not of type land");
                return null;
            }

            landTitleInstance = tile.TileInstance as LandTitleInstance;
        }

        if (landTitleInstance == null)
        {
            return null;
        }

        Action action = () => { GiveLand(gameManager, landTitleInstance);
            onEnd();
        };

        return new List<Action>() { action };
    }

    private void GiveLand(GameManager gameManager, LandTitleInstance landTitleInstance)
    {
        gameManager.CurrentPlayer.GainLand(landTitleInstance);
    }
}