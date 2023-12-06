using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEvent : Tile
{
    private EventTileDefinition _titleDefinition_Casted;

    public EventTileDefinition TitleDefinition_Casted => _titleDefinition_Casted;
    protected override void InitalizeTile()
    {
        base.InitalizeTile();
        if (titleDefinition is not EventTileDefinition)
        {
            Debug.LogError($"Invalid Cast Exepcted EventTileDefinition but it was {titleDefinition.name} ");
            return;
        }
        _titleDefinition_Casted = (EventTileDefinition)titleDefinition;
     
        // _meshRenderer.material = _titleDefinition_Casted.LandMaterial;
     
    }
}
