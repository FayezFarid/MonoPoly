using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileLand : Tile
{
    [SerializeField]  private MeshRenderer houseMesh;
    private SpriteRenderer _spriteRenderer;
    private LandTileDefinition titleDefinition_Casted;
    


    public LandTitleInstance LandTitleInstance => (LandTitleInstance)_tileInstance;
    public MeshRenderer HouseMesh=>houseMesh;

    
    public override void InitalizeTile()
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
        _tileInstance = new LandTitleInstance(titleDefinition_Casted,this);

        TileInstance.OnLandOwnerChanged += LandHasChanged;

    }

    private void LandHasChanged(LandTitleInstance titleInstance,Player Owner)
    {
        if (titleInstance != this._tileInstance)
        {
            return;
        }
        if (Owner == null)
        {
            _spriteRenderer.enabled = false;
            return;
        }
        _spriteRenderer.enabled = true;
        _spriteRenderer.color = Owner.PlayerColor;
        
    }
    //TODO use this maybe better than god forsken method justpass in gamemanager
    public override void PlayerPlacedOn(Player player)
    {
        throw new System.NotImplementedException();
    }
  
}
