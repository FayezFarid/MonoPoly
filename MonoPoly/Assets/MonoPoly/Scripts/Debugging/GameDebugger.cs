using System;
using System.Collections;
using System.Collections.Generic;
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
        if (GUI.Button(buttonRect, "Hide ", style))
        {
            HideGUI = true;
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

    private void RollDice()
    {
        // int x = Random.Range(50,1000);
        // int y = 0; /*Random.Range(0, 100);*/
        // int z = Random.Range(50, 1000);
        // diceRigibody.AddTorque((new Vector3(x, y, z)),ForceMode.Acceleration);
        // lastPos.y += 12;
        // diceRigibody.AddForce (new Vector3(x, y, z) * diceRigibody.mass,ForceMode.Acceleration);
        StartCoroutine(rollDiceCorotuine(ExpectedNumber));
    }

    private IEnumerator rollDiceCorotuine(int expectedValue)
    {
        float CurrentTime = 0;
        float Angle = 180;
        while (CurrentTime < 3)
        {
            // diceRigibody.AddTorque(new Vector3(110,10,10),ForceMode.Force);
            // diceRigibody.AddForce(new Vector3(110,10,10));
            int x = Random.Range(0, 2);
            int y = Random.Range(0, 2);
            int z = Random.Range(0, 2);

            diceRigibody.transform.Rotate(new Vector3(x, y, z), Angle);
            yield return null;
            CurrentTime += Time.deltaTime;
            Angle -= Angle * 0.01f * Time.deltaTime;
        }
        // diceRigibody.rotation=Quaternion.identity;
        //1
        // diceRigibody.Rotate(new Vector3(274f,270,270));

        diceRigibody.rotation = StaticDiceRotation[expectedValue - 1];

        yield return new WaitForSeconds(1f);
        Debug.Log($"Dice Value [{GetDiceValueRayCast()}]");
    }

    int GetDiceCount()
    {
        float[] resultArray = new float[6];
        float result;
        result = Vector3.Dot(diceRigibody.transform.forward, Vector3.up);
        resultArray[0] = result;
        if (result > 1)
            return 5;
        result = Vector3.Dot(-diceRigibody.transform.forward, Vector3.up);
        resultArray[1] = result;
        if (result > 1)
            return 2;
        result = Vector3.Dot(diceRigibody.transform.up, Vector3.up);
        resultArray[2] = result;
        if (result > 1)
            return 3;
        result = Vector3.Dot(-diceRigibody.transform.up, Vector3.up);
        resultArray[3] = result;
        if (result > 1)
            return 4;
        result = Vector3.Dot(diceRigibody.transform.right, Vector3.up);
        resultArray[4] = result;
        if (result > 1)
            return 6;
        result = Vector3.Dot(-diceRigibody.transform.right, Vector3.up);
        resultArray[5] = result;
        if (result > 1)
            return 1;
        SpicyHarissaLogger.LogErrorCritical("No Dice face????");
        return 0;
    }

    private int GetDiceValueRayCast()
    {
        Ray ray = new Ray();

        ray.direction = Camera.main.transform.position.GetDirection(diceRigibody.transform.position);
        ray.origin = Camera.main.transform.position;
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 3;
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 50, layerMask))
        {
            Debug.Log($"Hit smth name [ {hitInfo.collider.name} ]");
            return 0;
        }

        return 0;
        // Camera.main.
    }

    private void OnDrawGizmosSelected()
    {
        Ray ray = new Ray();
        ray.direction = Camera.main.transform.position.GetDirection(diceRigibody.transform.position);
        ray.origin = Camera.main.transform.position;
        Gizmos.DrawRay(ray);
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