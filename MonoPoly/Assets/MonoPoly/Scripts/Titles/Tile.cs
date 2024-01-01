using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TitleType
{
    Land,
    Prison,
    SpecialEvent,
    Station,
    Start,
    Parking,
}

public abstract class Tile : MonoBehaviour
{
    public static Action<Tile> OnTileClicked;
    //use this to know to what type to cast.
    [SerializeField] protected TitleType titleType;
    [SerializeField] protected TileDefinition titleDefinition;
    [SerializeField] private int positionInMap;
    
    protected MeshRenderer _meshRenderer;
    protected TileInstance _tileInstance;
    private EventTrigger _eventTrigger;


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

    private void Start()
    {
        _eventTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener(TileClicked);
        _eventTrigger.triggers.Add(entry);
    }

    public void TileClicked(BaseEventData eventData)
    {
        // SpicyHarissaLogger.Log("Tile was clicked",LogLevel.Verbose);
        OnTileClicked?.Invoke(this);
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
    //
    // private void Start()
    // {
    //     InitalizeTile();
    // }

    public virtual void InitalizeTile()
    {
        //TODO:Name of tile
        
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _meshRenderer.material = TitleDefinition.LandMaterial;
    }
    
    public virtual void PlayerPlacedOn(Player player)
    {
    }
    public void SetTileMatColor(Color32 color32)
    {
        _meshRenderer.material.color = color32;
    }
   
}