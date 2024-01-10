using System;
using UnityEngine;

public class StationTileInstance : LandTitleInstance
{
    public override int GetRent
    {
        get
        {
            int ownedStationCount = Owner.OwnedStations;
            if (ownedStationCount == 0)
                return LandDef.Rent[0];
            return LandDef.Rent[ownedStationCount - 1];
        }
    }

    public StationTileInstance(TileDefinition landdef, TileLand tileLand) : base(landdef, tileLand)
    {
    }
}