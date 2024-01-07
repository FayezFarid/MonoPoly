using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEvent : Tile
{
    private EventTileDefinition _titleDefinition_Casted;

    public EventTileDefinition TitleDefinition_Casted => _titleDefinition_Casted;
    public override void InitalizeTile()
    {
        base.InitalizeTile();
        if (titleDefinition is not EventTileDefinition)
        {
            Debug.LogError($"Invalid Cast Exepcted EventTileDefinition but it was {titleDefinition.name} ");
            return;
        }
        _titleDefinition_Casted = (EventTileDefinition)titleDefinition;
        if (titleType == TitleType.Parking)
        {
            _tileInstance = new ParkingTileInstance();
        }
        // _meshRenderer.material = _titleDefinition_Casted.LandMaterial;
     
    }
}
