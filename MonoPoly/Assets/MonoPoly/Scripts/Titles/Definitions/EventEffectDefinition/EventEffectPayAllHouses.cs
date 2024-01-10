using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Monopoly/ Event Effect Definition/Pay All houses")]
public class EventEffectPayAllHouses : EventEffectDefinition
{
    [SerializeField] private int valueForStandardHouse;
    [Header("Should be greater than Standard *4")]
    [SerializeField] private int valueForMaxHouse;
    
    public override List<Action> GetActions(GameManager gameManager, Action onEnd)
    {
       
        //Rememeber to put it in parking
        Action action = (() =>
        {
            PayAccordingToHouses(gameManager);
            onEnd();
        });
        return new List<Action>() { action };

    }

    private void PayAccordingToHouses(GameManager gameManager)
    {
        int TotalSum = 0;
        foreach (var keyPairedValue in gameManager.TileManager.LandInstancePairedLandType)
        {
            foreach (var land in keyPairedValue.Value)
            {
                if (land.AtMaxRank)
                {
                    TotalSum += valueForMaxHouse;
                    continue;
                }

                TotalSum += valueForStandardHouse * land.CurrentRank;
            }
        }
        gameManager.CurrentPlayer.PayPlayer(gameManager.TileManager.ParkingTileInstance,TotalSum);
    }
}