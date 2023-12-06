using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monopoly/MatchSettings")]
public class MatchSettings : ScriptableObject
{
    [SerializeField] private GameObject playerPrefab;

    public Color32[] PlayerPossibleColors;
    
    [SerializeField] private int initalMoney = 2000;
    [SerializeField] private int wholeLoopMoney = 200;
    [SerializeField]  private int playersNumber;
    [SerializeField] private int maxRoll;

    public GameObject PlayerPrefab => playerPrefab;
    public int WholeLoopMoney => wholeLoopMoney;
    
    public int InitalMoney => initalMoney;
    public int PlayersNumber => playersNumber;
}
