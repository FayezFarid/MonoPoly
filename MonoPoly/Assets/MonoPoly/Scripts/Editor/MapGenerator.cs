using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using Object = UnityEngine.Object;


public class MapGenerator : EditorWindow
{
    public static Vector3[] SpriteRenderPositions = new Vector3[4]
        { Vector3.zero, new Vector3(0.75f, 0, 0), new Vector3(-0.75F, 0, 0), new Vector3(0, 0, -0.75f) };

    public static Vector3[] HouseRenderPositions = new Vector3[4]
        { Vector3.zero, new Vector3(0.25f, 0, 0), new Vector3(-0.25f, 0, 0), new Vector3(0, 0, -0.25f) };

    public static int[] HouseRenderAngles = new int[4] { 0, 90, 270, 180 };


    public GameObject EventTilePrefab;
    public GameObject LandPreFabTile;
    public GameObject StationPreFabTile;

    public GameObject Parent;

    public Vector3 TileSize = new Vector3(4, 0.1f, 4);
    public Vector3 TileSizeHorz = new Vector3(5, 0.1f, 7);
    public Vector3 CornerTileSize;

    public Vector3 startingPosition = Vector3.zero;

    public Lane Lanes;

    public int Length;

    public int StartingIndex = 0;

    public int LanePosition = 0;

    [MenuItem("SpicyHarissa/BoardGenerator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(MapGenerator));
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
        EditorGUILayout.Space();
        Parent = (GameObject)EditorGUILayout.ObjectField("Tiles parent", Parent, typeof(GameObject), true);

        startingPosition = EditorGUILayout.Vector3Field("Starting Position", startingPosition);

        EditorGUILayout.Space();

        TileSize = EditorGUILayout.Vector3Field("Tile Size", TileSize);
        TileSizeHorz = EditorGUILayout.Vector3Field("Tile Size horztional", TileSizeHorz);
        CornerTileSize = EditorGUILayout.Vector3Field("Corner tile size", CornerTileSize);

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
            CreateBoardNew();
        }
    }


    public void CreateLane()
    {
        // CreateALane(ref startingPosition, LanePosition, Parent, StartingIndex);
    }

    public void CreateBoardNew()
    {
        Vector3 position = startingPosition;
        int startingIndex = 0;

        GameObject firstLaneParent = new GameObject("First Lane");
        firstLaneParent.transform.position = new Vector3(0, 0.1f, 0);
        CreateALane(ref position, 0, firstLaneParent, startingIndex);
        Debug.Log($"First position = {position}");

        //IDk why but this seems to work
        position.z +=  TileSizeHorz.z - 0.5f;
        position.x += CornerTileSize.x;
        
        startingIndex += 10;
        GameObject secondLaneParent = new GameObject("Second Lane");
        secondLaneParent.transform.position = new Vector3(0, 0.1f, 0);
        ;
        CreateALane(ref position, 1, secondLaneParent, startingIndex);

        position.z -= CornerTileSize.z;
        position.x += CornerTileSize.x - 0.5f;

        Debug.Log($"Second position = {position}");

        startingIndex += 10;
        GameObject thirdLaneParent = new GameObject("Third Lane");
        thirdLaneParent.transform.position = new Vector3(0, 0.1f, 0);
        CreateALane(ref position, 2, thirdLaneParent, startingIndex);

        position.z -= CornerTileSize.z-0.5f;
        position.x -= CornerTileSize.x;
        Debug.Log($"Third position = {position}");

        startingIndex += 10;
        GameObject fourthLaneParent = new GameObject("Fourth Lane");
        fourthLaneParent.transform.position = new Vector3(0, 0.1f, 0);
        CreateALane(ref position, 3, fourthLaneParent, startingIndex);
        Debug.Log($"Fourth position = {position}");
    }


    private bool IsHorzIndex(int index) => index is 1 or 3;

    private int GetSign(int index)
    {
        if (index is 0 or 3)
            return -1;
        return 1;
    }

    public void CreateALane(ref Vector3 StartingPosition, int laneIndex, GameObject parent, int additonalIndex)
    {
        Lanes.OnAfterDeserialize();

        for (int i = 0; i < 10; i++)
        {
            TileDefinition tileDefinition = Lanes.LanesDefinitions[laneIndex][i];
            Object obj = PrefabUtility.InstantiatePrefab(GetCorrespendingObject(tileDefinition));
            GameObject TileGO = (GameObject)obj;

            float valueToAdd = i == 9 ? CornerTileSize.x : TileSizeHorz.x;
            valueToAdd *= GetSign(laneIndex);
            Vector3 tileSize = i == 9 ? CornerTileSize : TileSizeHorz;

            SetGameObjectStandard(TileGO, parent.transform, ref StartingPosition, valueToAdd, !IsHorzIndex(laneIndex),
                tileSize);

            if (i != 9 && IsHorzIndex(laneIndex))
                TileGO.transform.Rotate(Vector3.up, 90);
            Tile tile = TileGO.GetComponent<Tile>();

            SetupInformation(tile, tileDefinition, i + additonalIndex);
            SetTileType(tile, tileDefinition);

            if (tile.TitleType == TitleType.Land)
            {
                TileLand tileLand = (TileLand)tile;
                LandSpriteAndHouses(TileGO, tileLand, laneIndex);
            }
        }
    }

    private void LandSpriteAndHouses(GameObject tileGO, TileLand tileLand, int laneIndex)
    {
        SetSpriteRender(tileGO, SpriteRenderPositions[laneIndex]);
        SetHouse(tileLand, HouseRenderPositions[laneIndex], HouseRenderAngles[laneIndex]);
    }


    private void SetGameObjectStandard(GameObject TileGO, Transform parent, ref Vector3 position, float valueToAdd,
        bool AddToX /*If NotX It's Z*/, Vector3 tileSize)
    {
        TileGO.transform.position = position;
        // if (AddToX)
        // {

        TileGO.transform.localScale = tileSize;
        // }
        // else
        // {
        //     TileGO.transform.localScale = TileSizeHorz;
        // }

        if (AddToX)
        {
            position.x += valueToAdd;
        }
        else
        {
            // TileGO.transform.Rotate(Vector3.up,90);
            position.z += valueToAdd;
        }

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

    private void SetHouse(TileLand tileLand, Vector3 Pos, float RotationAngle)
    {
        Transform transform = tileLand.HouseMesh.transform;

        if (Pos != Vector3.zero)
            transform.localPosition = Pos;
        if (RotationAngle != 0)
            transform.Rotate(new Vector3(0, 1, 0), RotationAngle);


        Vector3 localPos = transform.localScale;
        localPos.y = 3;
        transform.localScale = localPos;
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
}

// public MapGeneratorInspector:Editor