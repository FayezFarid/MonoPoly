using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileLand : Tile
{
    private LandTileDefinition titleDefinition_Casted;

    public LandTitleInstance LandTitleInstance => (LandTitleInstance)_tileInstance;

    private SpriteRenderer _spriteRenderer;
    
    protected override void InitalizeTile()
    {
        base.InitalizeTile();
        if (titleDefinition is not LandTileDefinition)
        {
            Debug.LogError($"Invalid Cast Exepcted LandTileDefinition but it was {titleDefinition.name} ");
            return;
        }
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        titleDefinition_Casted = (LandTileDefinition)titleDefinition;
        //TODO:Name of tile
        
    
        // _meshRenderer.material = titleDefinition_Casted.LandMaterial;
        _tileInstance = new LandTitleInstance(titleDefinition_Casted);

        _tileInstance.OnLandOwnerChanged += LandHasChanged;

    }

    private void LandHasChanged(Player Owner)
    {
        if (Owner == null)
        {
            _spriteRenderer.enabled = false;
            return;
        }
        _spriteRenderer.enabled = true;
        _spriteRenderer.color = Owner.PlayerColor;
        
    }
    public override void PlayerPlacedOn(Player player)
    {
        throw new System.NotImplementedException();
    }
}
