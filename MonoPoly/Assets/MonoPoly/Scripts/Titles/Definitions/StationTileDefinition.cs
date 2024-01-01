using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monopoly/Station Tile Definition")]
public class StationTileDefinition : LandTileDefinition
{
    public static readonly int[] Station_Default_Rent = new[] { 25, 50, 100, 200 };

    public override bool CanSellHouses => false;
    public override int MoneyToUpgrade => 0;
    public override int[] Rent => Station_Default_Rent;
}
