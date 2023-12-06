using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TitleType
{
    Land,
    Prison,
    SpecialEvent,
    Station,
    Start
}

public abstract class Tile : MonoBehaviour
{
    //use this to know to what type to cast.
    [SerializeField] protected TitleType titleType;
    protected MeshRenderer _meshRenderer;
    
    [SerializeField] protected TileDefinition titleDefinition;
    
    protected TileInstance _tileInstance;


    [SerializeField] private int positionInMap;


    public TileInstance TileInstance => _tileInstance;

    public TileDefinition TitleDefinition
    {
        get { return titleDefinition; }
#if UNITY_EDITOR
        set => titleDefinition = value;
#endif
    }

    public int PositionInMap
    {
        get { return positionInMap; }
#if UNITY_EDITOR
        set => positionInMap = value;
#endif
    }

    public TitleType TitleType
    {
        get => titleType;
        set => titleType=value;
    }

    public void PlacePlayer(Transform ObjectToPlace)
    {
        ObjectToPlace.position = transform.position;
    }

    private void Start()
    {
        InitalizeTile();
    }

    protected virtual void InitalizeTile()
    {
        //TODO:Name of tile
        
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _meshRenderer.material = TitleDefinition.LandMaterial;
    }
    
    public virtual void PlayerPlacedOn(Player player)
    {
    }
}