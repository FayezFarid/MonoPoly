using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "SpicyHarissa/ Lane")]
public class Lane : ScriptableObject
{
    [FormerlySerializedAs("Definitions")] public List<TileDefinition> Definitions_firstLane;
    public List<TileDefinition> Definitions_secondLane;
    public List<TileDefinition> Definitions_thirdLane;
    public List<TileDefinition> Definitions_fourthLane;

    private List<List<TileDefinition>> lanesDefinitions = new List<List<TileDefinition>>();

    public List<List<TileDefinition>> LanesDefinitions
    {
        get
        {
     
            return lanesDefinitions;
        }
    }
    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        lanesDefinitions = new List<List<TileDefinition>>();
        LanesDefinitions.Add(Definitions_firstLane);
        LanesDefinitions.Add(Definitions_secondLane);
        LanesDefinitions.Add(Definitions_thirdLane);
        LanesDefinitions.Add(Definitions_fourthLane);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Lane))]
public class LaneInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Reverse FirstLane"))
        {
            Lane thisTarget= target as Lane;
            thisTarget.Definitions_firstLane.Reverse();
        }
    }
}
#endif