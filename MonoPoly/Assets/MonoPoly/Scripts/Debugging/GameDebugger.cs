using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class GameDebugger : MonoBehaviour
{
    public static Quaternion[] StaticDiceRotation = new Quaternion[6]
    {
        new Quaternion(-0.6f, 0, 0, 0.7f),
        Quaternion.identity,
        new Quaternion(-0.005f, 0.001f, -0.7f, 0.7f),
        new Quaternion(0, 0, 0.68f, 0.72f),
        new Quaternion(1, 0, 0, 0),
        new Quaternion(0.707f, 0, 0, 0.707f),
    };

    public Player TargetPlayer;
    public GameManager GameManager;

    [SerializeField] private bool HideGUI = false;


    public GUIStyle style;
    public GUIStyle SideBarStyle;
    public GUIStyle BackGround;

    public Vector2 BoxPosition;
    public Vector2 BoxSize = new Vector2(500, 600);
    public Vector2 StandardButtonSize = new Vector2(100, 300);
    public Vector2 StandardExtraOption = new Vector2(100, 100);
    public int ExpectedNumber;

    //Arguments

    public int positionToPlacePlayer;
    private string PositionToPlaceResultString;

    public int LandIndex;
    private string LandIndexResultString;

    public int FixedDiceValue;
    private string FixedDiceValueString;

    private Vector2 _newSideBarPosition;
    private Vector2 _newButtonPosition;

    [SerializeField] private Transform diceRigibody;


    public float firstLineDis;
    public float secondLineDis;
    public float thirdLineDis;
    public float fourthLineDis;

    public float firstLineAngle;
    public float secondLineAngle;
    public float thirdLineAngle;
    public float fourthLineAngle;


    public void OnGUI()
    {
        // if(BoxPosition==Vector2.zero)
        //     BoxPosition = new Vector2(-(Screen.width / 4), Screen.width / 2);

        if (HideGUI)
        {
            DrawHiddenGUI();
            return;
        }

        Rect boxRect = new Rect(BoxPosition, BoxSize);
        GUI.Box(boxRect, "Debugging ", BackGround);

        _newButtonPosition = new Vector2();
        _newButtonPosition = BoxPosition;
        // buttonPosition.x = +200;
        _newButtonPosition.y = +50;
        Rect buttonRect = new Rect(_newButtonPosition, StandardButtonSize);
        _newButtonPosition.x += StandardButtonSize.x;
        Rect ExtraHeaderRect = new Rect(_newButtonPosition, StandardExtraOption);


        PositionToPlaceResultString = GUI.TextField(ExtraHeaderRect, PositionToPlaceResultString, SideBarStyle);
        if (!string.IsNullOrEmpty(PositionToPlaceResultString))
        {
            positionToPlacePlayer = int.Parse(PositionToPlaceResultString);
        }

        if (GUI.Button(buttonRect, "place position", style))
        {
            if (TargetPlayer != null)
                GameManager.PlacePlayer(TargetPlayer, positionToPlacePlayer);
            else
                GameManager.PlacePlayer(GameManager.TurnManager.CurrentPlayerTurn, positionToPlacePlayer);
        }

        Vector2 buttonPositionGiveLand = buttonRect.position;
        _newButtonPosition = buttonPositionGiveLand;
        _newButtonPosition.y += StandardButtonSize.y;
        buttonRect.position = _newButtonPosition;
        if (GUI.Button(buttonRect, "Give Player Land", style))
        {
            if (TargetPlayer)
                PlayerBuyLand(TargetPlayer);
            else PlayerBuyLand(GameManager.TurnManager.CurrentPlayerTurn);
        }

        _newSideBarPosition = ExtraHeaderRect.position;
        ExtraHeaderRect.y += StandardExtraOption.y;

        buttonRect.position = _newButtonPosition;
        LandIndexResultString = GUI.TextField(ExtraHeaderRect, LandIndexResultString, SideBarStyle);
        if (!string.IsNullOrEmpty(LandIndexResultString))
        {
            LandIndex = int.Parse(LandIndexResultString);
        }

        _newButtonPosition.y += StandardButtonSize.y;
        buttonRect.position = _newButtonPosition;
        if (GUI.Button(buttonRect, "Fix Dice value", style))
        {
            // GameManager.Dice.FixedDiceValue = FixedDiceValue;

            GameManager.DiceManager.FixedValue = FixedDiceValue;
            GameManager.DiceManager.FixValue = FixedDiceValue != 0;
        }

        _newSideBarPosition = ExtraHeaderRect.position;
        ExtraHeaderRect.y += StandardExtraOption.y;

        buttonRect.position = _newButtonPosition;
        FixedDiceValueString = GUI.TextField(ExtraHeaderRect, FixedDiceValueString, SideBarStyle);
        if (!string.IsNullOrEmpty(FixedDiceValueString))
        {
            FixedDiceValue = int.Parse(FixedDiceValueString);
        }

        _newButtonPosition.y += StandardButtonSize.y;
        buttonRect.position = _newButtonPosition;
        if (GUI.Button(buttonRect, "Turn player broke", style))
        {
            if (TargetPlayer)
                TargetPlayer.ReduceMoney(TargetPlayer.Money);
            else GameManager.TurnManager.CurrentPlayerTurn.ReduceMoney(GameManager.TurnManager.CurrentPlayerTurn.Money);
        }

        _newButtonPosition.y += StandardButtonSize.y;
        buttonRect.position = _newButtonPosition;
        if (GUI.Button(buttonRect, "Give player 100 ", style))
        {
            if (TargetPlayer)
                TargetPlayer.IncreaseMoney(100);
            else GameManager.TurnManager.CurrentPlayerTurn.IncreaseMoney(100);
        }

        _newButtonPosition.y += StandardButtonSize.y;
        buttonRect.position = _newButtonPosition;
        if (GUI.Button(buttonRect, "Next Turn", style))
        {
            GameManager.NextPlayerturn();
        }

        _newButtonPosition.y += StandardButtonSize.y;
        buttonRect.position = _newButtonPosition;
        if (GUI.Button(buttonRect, "Give current Player all lands", style))
        {
            GivePlayerAllLand();
        }

        _newButtonPosition.y += StandardButtonSize.y;
        buttonRect.position = _newButtonPosition;
        if (GUI.Button(buttonRect, "Hide ", style))
        {
            HideGUI = true;
        }
    }

    private void GivePlayerAllLand()
    {
        Player currentPlayer = GameManager.CurrentPlayer;
        foreach (var KeyPairedValue in GameManager.TileManager.LandInstancePairedLandType)
        {
            foreach (var land in KeyPairedValue.Value)
            {
                currentPlayer.GainLand(land);
            }
        }
    }

    public void DrawHiddenGUI()
    {
        _newButtonPosition = new Vector2();
        _newButtonPosition = BoxPosition;
        // buttonPosition.x = +200;
        _newButtonPosition.y = +50;
        Rect buttonRect = new Rect(_newButtonPosition, StandardButtonSize);
        if (GUI.Button(buttonRect, "Show ", style))
        {
            HideGUI = false;
        }
    }

    private Vector3 lastPos;
    private Vector3 initPos;


    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            return;
        }
#endif

        Vector3 Tile0 = GameManager.TileManager[39].transform.position;
        Quaternion tile0Angle = GameManager.TileManager[39].transform.rotation;
        Vector3 tile9 = GameManager.TileManager[9].transform.position;
        Quaternion tile9Angle = GameManager.TileManager[9].transform.rotation;
        firstLineDis = Vector3.Distance(Tile0, tile9);
        firstLineAngle = Quaternion.Angle(tile0Angle, tile9Angle);
        Gizmos.DrawLine(Tile0, tile9);

        Vector3 Tile19 = GameManager.TileManager[19].transform.position;
        Quaternion tile19Angle = GameManager.TileManager[19].transform.rotation;
        secondLineAngle = Quaternion.Angle(tile9Angle, tile19Angle);
        secondLineDis = Vector3.Distance(tile9, Tile19);
        Gizmos.DrawLine(tile9, Tile19);

        Vector3 Tile29 = GameManager.TileManager[29].transform.position;
        Quaternion tile29Angle = GameManager.TileManager[29].transform.rotation;
        thirdLineDis = Vector3.Distance(Tile19, Tile29);
        thirdLineAngle = Quaternion.Angle(tile19Angle, tile29Angle);
        Gizmos.DrawLine(Tile19, Tile29);

        // Vector3 Tile29 = GameManager.TileManager[29].transform.position;
        fourthLineDis = Vector3.Distance(Tile29, Tile0);
        fourthLineAngle = Quaternion.Angle(tile29Angle, tile0Angle);
        Gizmos.DrawLine(Tile29, Tile0);
    }

    private void PlayerBuyLand(Player player)
    {
        Tile tile = GameManager.TileManager.TitleOrganized[LandIndex];
        if (tile is TileLand)
        {
            TileLand tileLand = (TileLand)tile;
            player.BuyLand(tileLand.LandTitleInstance);
        }
    }
}