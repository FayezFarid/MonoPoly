using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;


public class MapGenerator : EditorWindow
{
    [MenuItem("SpicyHarissa/BoardGenerator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MapGenerator));
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("so far only I know how this work, soon only god will know");
        EditorGUILayout.Space();
        EventTilePrefab =
            (GameObject)EditorGUILayout.ObjectField("EventTilePrefab", EventTilePrefab, typeof(GameObject), true);
        LandPreFabTile =
            (GameObject)EditorGUILayout.ObjectField("LandPreFabTile", LandPreFabTile, typeof(GameObject), true);
        StationPreFabTile =
            (GameObject)EditorGUILayout.ObjectField("StationPreFabTile", StationPreFabTile, typeof(GameObject), true);
        Parent = (GameObject)EditorGUILayout.ObjectField("Tiles parent", Parent, typeof(GameObject), true);

        startingPosition = EditorGUILayout.Vector3Field("Starting Position", startingPosition);
        TileSize = EditorGUILayout.Vector3Field("Tile Size", TileSize);

        Lanes = (Lane)EditorGUILayout.ObjectField("Lane", Lanes, typeof(Lane), true);

        Length = EditorGUILayout.IntField("Lane Length", Length);
        EditorGUILayout.LabelField("Starting index for example if it's second row this would be 9  third row 9+9 etc");
        StartingIndex = EditorGUILayout.IntField("Starting index", StartingIndex);
        EditorGUILayout.LabelField("0= bottom 1= middle left  2= top 3= middle right");
        LanePosition = EditorGUILayout.IntField("Lane position", LanePosition);
        if (GUILayout.Button("CreateLane"))
        {
            CreateLane();
        }

        if (GUILayout.Button("Create Board"))
        {
            CreateBoard();
        }
    }

    public GameObject EventTilePrefab;
    public GameObject LandPreFabTile;
    public GameObject StationPreFabTile;

    public GameObject Parent;

    public Vector3 TileSize = new Vector3(4, 0.1f, 4);

    public Vector3 startingPosition = Vector3.zero;

    public Lane Lanes;

    public int Length;

    public int StartingIndex = 0;

    public int LanePosition = 0;

    public void CreateLane()
    {
        if (LanePosition == 0)
        {
            CreateLaneBottom(startingPosition, Parent, StartingIndex);
            return;
        }
        else if (LanePosition == 1)
        {
            CreateLaneMiddleLeft(startingPosition, Parent, StartingIndex);
        }
        else if (LanePosition == 2)
        {
            CreateLaneTop(startingPosition, Parent, StartingIndex);
        }
        else if (LanePosition == 3)
        {
            CreateLaneMiddleRight(startingPosition, Parent, StartingIndex);
        }
    }

    public void CreateBoard()
    {
        Vector3 position = startingPosition;
        int startingIndex = 0;

        GameObject firstLaneParent = new GameObject("First Lane");
        position = CreateLaneBottom(position, firstLaneParent, startingIndex);
        Debug.Log($"First position = {position}");

        startingIndex += 10;
        GameObject secondLaneParent = new GameObject("Second Lane");
        position = CreateLaneMiddleLeft(position, secondLaneParent, startingIndex);

        position.z -= TileSize.z;
        position.x += TileSize.x;
        
        Debug.Log($"Second position = {position}");

        startingIndex += 10;
        GameObject thirdLaneParent = new GameObject("Third Lane");
        position = CreateLaneTop(position, thirdLaneParent, startingIndex);

        position.z -= TileSize.z;
        position.x -= TileSize.x;
        Debug.Log($"Third position = {position}");

        startingIndex += 10;
        GameObject fourthLaneParent = new GameObject("Fourth Lane");
        position = CreateLaneMiddleRight(position, fourthLaneParent, startingIndex);
        Debug.Log($"Fourth position = {position}");
    }

    private GameObject GetCorrespendingObject(TileDefinition tileDefinition)
    {
        if (tileDefinition is EventTileDefinition)
        {
            return EventTilePrefab;
        }

        if (tileDefinition is LandTileDefinition)
        {
            return LandPreFabTile;
        }

        if (tileDefinition is StationTileDefinition)
        {
            return StationPreFabTile;
        }

        return null;
    }

    public Vector3 CreateLaneMiddleLeft(Vector3 StartingPosition, GameObject parent, int AdditonalIndex)
    {
        Vector3 position = StartingPosition;
        for (int i = 0; i < 10; i++)
        {
            TileDefinition tileDefinition = Lanes.Definitions_secondLane[i];
            GameObject TileGO = Instantiate(GetCorrespendingObject(tileDefinition));


            // TileGO.transform.position = position;
            // TileGO.transform.localScale = TileSize;
            // position.z += TileSize.z;
            // TileGO.transform.SetParent(parent.transform);

            SetGameObjectStandard(TileGO, parent.transform,  ref position, TileSize.z, false);

            Tile tile = TileGO.GetComponent<Tile>();
            SetupInformation(tile, tileDefinition, i + AdditonalIndex);

            if (tile.TitleType == TitleType.Land)
            {
                SetSpriteRender(TileGO, new Vector3(0.75f, 0, 0));
            }

            SetTileType(tile, tileDefinition);
        }

        return position;
    }

    public Vector3 CreateLaneMiddleRight(Vector3 StartingPosition, GameObject parent, int AdditonalIndex)
    {
        Vector3 position = StartingPosition;
        for (int i = 0; i < 10; i++)
            // for (int i = 9; i >= 0; i--)
        {
            TileDefinition tileDefinition = Lanes.Definitions_fourthLane[i];
            GameObject TileGO = Instantiate(GetCorrespendingObject(tileDefinition));


            SetGameObjectStandard(TileGO, parent.transform,ref position, -TileSize.z, false);

            Tile tile = TileGO.GetComponent<Tile>();
            SetupInformation(tile, tileDefinition, i + AdditonalIndex);

            if (tile.TitleType == TitleType.Land)
            {
                SetSpriteRender(TileGO, new Vector3(-0.75F, 0, 0));
            }

            SetTileType(tile, tileDefinition);
        }

        return position;
    }


    public Vector3 CreateLaneTop(Vector3 StartingPosition, GameObject parent, int AdditonalIndex)
    {
        Vector3 position = StartingPosition;
        for (int i = 0; i < 10; i++)
        {
            TileDefinition definition = Lanes.Definitions_thirdLane[i];
            GameObject TileGO = Instantiate(GetCorrespendingObject(definition));


            SetGameObjectStandard(TileGO, parent.transform,ref position, TileSize.x, true);

            Tile tile = TileGO.GetComponent<Tile>();
            SetupInformation(tile, definition, i + AdditonalIndex);

            if (tile.TitleType == TitleType.Land)
            {
                SetSpriteRender(TileGO, new Vector3(0, 0, -0.75f));
            }

            SetTileType(tile, definition);
        }

        return position;
    }

    public Vector3 CreateLaneBottom(Vector3 StartingPosition, GameObject parent, int AdditonalIndex)
    {
        Vector3 position = StartingPosition;
        for (int i = 9; i >= 0; i--)
        {
            TileDefinition tileDefinition = Lanes.Definitions_firstLane[i];
            GameObject TileGO = Instantiate(GetCorrespendingObject(tileDefinition));


            SetGameObjectStandard(TileGO, parent.transform,ref position, TileSize.x, true);

            Tile tile = TileGO.GetComponent<Tile>();
            SetupInformation(tile, tileDefinition, i + AdditonalIndex);


            SetTileType(tile, tileDefinition);
        }

        position = StartingPosition;
        position.z = TileSize.z;
        return position;
    }

    private void SetGameObjectStandard(GameObject TileGO, Transform parent, ref Vector3 position, float valueToAdd,
        bool AddToX /*If NotX It's Z*/)
    {
        TileGO.transform.position = position;
        TileGO.transform.localScale = TileSize;

        if (AddToX)
            position.x += valueToAdd;
        else
            position.z += valueToAdd;

        TileGO.transform.SetParent(parent);
    }

    private void SetSpriteRender(GameObject TileGO, Vector3 localPos)
    {
        SpriteRenderer spriteRenderer = TileGO.GetComponentInChildren<SpriteRenderer>();
        Vector2 size = spriteRenderer.size;
        // float Xpos = (float)-0.5 + size.x / 2;
        spriteRenderer.transform.localPosition = localPos;
    }

    private void SetTileType(Tile tile, TileDefinition tileDefinition)
    {
        if (tileDefinition is StationTileDefinition)
        {
            tile.TitleType = TitleType.Station;
            return;
        }

        if (tileDefinition is LandTileDefinition)
        {
            tile.TitleType = TitleType.Land;
            return;
        }

        EventTileDefinition eventTileDefinition = (EventTileDefinition)tileDefinition;

        if (eventTileDefinition.eventType == EventType.Start)
        {
            tile.TitleType = TitleType.Start;
        }

        if (eventTileDefinition.eventType == EventType.Prison)
        {
            tile.TitleType = TitleType.Prison;
        }
    }

    private void SetupInformation(Tile tile, TileDefinition tileDefinition, int Index)
    {
        tile.PositionInMap = Index;
        tile.TitleDefinition = tileDefinition;

        tile.gameObject.name = $"Tile_{tile.PositionInMap}_{tile.TitleDefinition.name}";
    }
}

// public MapGeneratorInspector:Editor