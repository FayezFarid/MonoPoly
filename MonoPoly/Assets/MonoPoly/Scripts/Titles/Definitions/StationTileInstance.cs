using System;
using UnityEngine;

public class StationTileInstance : LandTitleInstance
{
    public override int GetRent
    {
        get
        {
            int ownedStationCount = Owner.OwnedStations;
            return LandDef.Rent[ownedStationCount - 1];
        }
    }

    public StationTileInstance(TileDefinition landdef, TileLand tileLand) : base(landdef, tileLand)
    {
    }
}