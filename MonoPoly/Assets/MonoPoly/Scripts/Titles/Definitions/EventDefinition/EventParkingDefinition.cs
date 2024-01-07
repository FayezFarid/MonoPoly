using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Monopoly/ Event Parking  Definition")]
public class EventParkingDefinition : EventDefinition
{
    public override List<Action> GetActions(GameManager gameManager)
    {
        //Predam fix it
       
        void Action()
        {
            //this to current player
            ParkingTileInstance parkingTileInstance = (ParkingTileInstance) gameManager.TileManager.ParkingTile.TileInstance;
            ExtensionFillEventEffects.PlayerGivesMoneyTo(parkingTileInstance,gameManager.CurrentPlayer,parkingTileInstance.Money);
        }
        return new List<Action>()
        {
            Action
        };
    }

}
