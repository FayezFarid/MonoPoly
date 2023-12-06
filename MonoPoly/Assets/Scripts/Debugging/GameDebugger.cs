using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDebugger : MonoBehaviour
{

    public Player TargetPlayer;
    public GameManager GameManager;

    public int positionToPlacePlayer;
    public Vector2 BoxPosition;
    public Vector2 BoxSize= new Vector2(500,600);
    public Vector2 StandardButtonSize = new Vector2(100, 500);
    public void OnGUI()
    {
        // if(BoxPosition==Vector2.zero)
        //     BoxPosition = new Vector2(-(Screen.width / 4), Screen.width / 2);

        
        Rect boxRect = new Rect(BoxPosition,BoxSize);
        Vector2 buttonPosition = new Vector2();
        buttonPosition = BoxPosition;
        // buttonPosition.x = +200;
        buttonPosition.y = +200;
        Rect buttonRect = new Rect(buttonPosition,StandardButtonSize);
        GUI.Box(boxRect,"Debugging ");
        if (GUI.Button(buttonRect, "place position"))
        {
            GameManager.PlacePlayer(TargetPlayer,positionToPlacePlayer);
        }
    }
}
