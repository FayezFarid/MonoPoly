using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "SpicyHarissa/ Lane")]
public class Lane: ScriptableObject
{
    [FormerlySerializedAs("Definitions")] public List<TileDefinition> Definitions_firstLane;
    public List<TileDefinition> Definitions_secondLane;
    public List<TileDefinition> Definitions_thirdLane;
    public List<TileDefinition> Definitions_fourthLane;
    
}