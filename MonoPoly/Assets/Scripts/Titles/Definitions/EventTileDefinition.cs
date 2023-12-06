using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class TileDefinition : ScriptableObject
{
    [SerializeField] protected Material landMaterial;
    public Material LandMaterial => landMaterial;
}

public enum EventType
{
    Roll,
    GoPrison,
    Prison,
    Start,
    RandomEffect,
}

[CreateAssetMenu(menuName = "Monopoly/ Event Tile Definition")]
public class EventTileDefinition : TileDefinition
{
    [SerializeField] public EventType eventType;
    [SerializeField] private List<EventDefinition> eventDefinitions;

    public EventDefinition GetEvent()
    {
        if (eventDefinitions.Count == 0)
        { 
            SpicyHarissaLogger.LogErrorCritical($"EventTileDefinition eventDefinitions is empty");
            return null;
        }

        if (eventDefinitions.Count == 1)
        {
            return eventDefinitions.First();
        }

        int RandomIndex = Random.Range(0, eventDefinitions.Count);
        return eventDefinitions[RandomIndex];
    }
}



public abstract class TileInstance
{
    public abstract void OnLandedUpon();
    public  Action< Player> OnLandOwnerChanged;
}