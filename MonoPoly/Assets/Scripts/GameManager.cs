using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Titles

    private Dictionary<LandTypes, List<LandTitleInstance>> _landTitleInstances;
    
    #endregion


    //For Debugging
    //this suposedly read only.
    [SerializeField] private List<Player> allPlayers;

    private TileManager _tileManager;
    private TurnManager _turnManager;
    public TurnManager TurnManager => _turnManager;

    #region Editor Variables

    //prefabs
    [Header("Scene variables")] [SerializeField]
    private PlayerMenu playerMenu;

    //This is easier than MainCamera. as it's static scene
    [SerializeField]
    private Camera MainCamera;


    [SerializeField] private BuyLandMenu buyLandMenu;


    [SerializeField] private Dice dice;
    [SerializeField] private Rigidbody diceRigibody;
    [SerializeField] private int SpinDuration = 5;

    [SerializeField] private MatchSettings matchSettings;

    #endregion


    private void Start()
    {
        
        _tileManager = GetComponent<TileManager>();
        _tileManager.InitalizeWithGameManager(this);
        _tileManager.SortTitles();
        InitPlayers();
        StartCoroutine(SpinDice());

        foreach (Player playerItem in allPlayers)
        {
            PlacePlayer(playerItem, 0);
        }

        MainCamera.enabled = false;
        _turnManager.CurrentPlayerTurn.PlayerTurnStarted();
    }

    #region Initalize

    private void InitPlayers()
    {
        allPlayers = new List<Player>();
        for (int i = 0; i < matchSettings.PlayersNumber; i++)
        {
            Player player = CreatePlayer();
            player.PlayerName = $"Player {i}";
            player.IncreaseMoney(matchSettings.InitalMoney);
            player.PlayerColor = matchSettings.PlayerPossibleColors[i];
            allPlayers.Add(player);
        }

        _turnManager = new TurnManager(allPlayers);
        playerMenu.InitializeWithPlayers(allPlayers);
    }

    private Player CreatePlayer()
    {
        var gameobj = Instantiate(matchSettings.PlayerPrefab);
        return gameobj.GetComponent<Player>();
    }

    #endregion

    #region Turn managment

    public void NextPlayerturn()
    {
        MainCamera.enabled = false;
        _turnManager.CurrentPlayerTurn.PlayerTurnEnded();
        _turnManager.NextTurn();
        _turnManager.CurrentPlayerTurn.PlayerTurnStarted();
    }

    #endregion
    #region Player landed ...

    public void PlayerLanededOnEvent(Player player, EventTileDefinition eventTileDefinition)
    {
        EventDefinition eventDefinition = eventTileDefinition.GetEvent();
        if(eventDefinition==null)
            return;
        List<Action> actions= AttributeHandler.FillAction(eventDefinition,this);
        foreach (var item in actions)
        {
            item.Invoke();
        }
    }
    public void PlayerLandedOnStation(Player player, LandTitleInstance station)
    {
        //TODO:check station shid
        PlayerLandedOnLand(player,station);
    }
    public void PlayerLandedOnLand(Player player, LandTitleInstance landTitleInstance)
    {
        if (landTitleInstance.IsOwned)
        {
            //pay rent 
            PlayerPayRent(player,landTitleInstance);
        }
        else
        {
            //buy
            PlayerBuyLand(player,landTitleInstance);
        }
    }
    public void PlayerBuyLand(Player player, LandTitleInstance landTitleInstance)
    {
        //Todo:Add auction.
        buyLandMenu.EnableMenu(buyLandAction, StartAuction, landTitleInstance, player);
    }

    public void PlayerPayRent(Player player, LandTitleInstance landTitleInstance)
    {
       
        if(!player.PlayerOwnsLand(landTitleInstance))
            player.PayPlayer(landTitleInstance.Owner, landTitleInstance.GetRent);
    }

    private void StartAuction()
    {
        Debug.Log($"Start auction clicked");
    }

    private void buyLandAction(Player player, LandTitleInstance landTitleInstance)
    {
        player.BuyLand(landTitleInstance);
     
    }

    private void CurrentPlayerDidWholeLoop()
    {
        _turnManager.CurrentPlayerTurn.IncreaseMoney(matchSettings.WholeLoopMoney);
    }
    #endregion

    
    #region  Board movments
    
    /// <summary>
    /// Moves current Player . rolls dice etc
    /// </summary>
    public void MoveCurrentPlayer()
    {
        int DiceValue = RollDice();
        Debug.Log($"Rolled Dice Value = {DiceValue}");
        
        //Gotta return back to 0 
        int PositionToPlace = GetNextAvaliableTitle(_turnManager.CurrentPlayerTurn.CurrentPosition, DiceValue,
            _tileManager.TileCount);
        if (PositionToPlace < _turnManager.CurrentPlayerTurn.CurrentPosition)
        {
            CurrentPlayerDidWholeLoop();
        }
        PlacePlayer(_turnManager.CurrentPlayerTurn, PositionToPlace);
    }

    private int GetNextAvaliableTitle(int CurrentPosition, int DiceValue, int TotalLength)
    {
        int Difference = TotalLength - CurrentPosition;
        Difference -= 1;
        if (Difference < DiceValue)
        {
            Debug.Log($"Difference was bigger Going to return {Difference - DiceValue}");
            
            return Mathf.Abs(Difference - DiceValue);
        }

        Debug.Log($"Standard Position  {CurrentPosition + DiceValue}");
        return CurrentPosition + DiceValue;
        // if (CurrentPosition + DiceValue > allTitles.Length)
        // {
        //     
        // }
    }

    public int RollDice()
    {
        return dice.Roll();
    }

    public void PlacePlayer(Player PlayerToPlace, int position)
    {
        //divided whatever visual and triggering effect of where player landed
        _tileManager[position].PlacePlayer(PlayerToPlace.transform);
        PlayerToPlace.CurrentPosition = position;
        _tileManager.PlayerLandedOnTile(PlayerToPlace, position);
        PlayerToPlace.transform.LookAt(_tileManager[position+1].transform);
    }


    private IEnumerator SpinDice()
    {
        diceRigibody.AddTorque(Vector3.up * 1000, ForceMode.Impulse);
        // lastPos.y += 12;
        // diceObject.rigidbody.AddForce (((lastPos - initPos).normalized) * (Vector3.Distance (lastPos, initPos)) * 25 * duceObject.rigidbody.mass);

        float CurrentTime = 0;
        Vector3[] Directionss = new[] { Vector3.down, Vector3.up, Vector3.right, };
        while (CurrentTime < SpinDuration)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            CurrentTime += Time.deltaTime;
            // int RandomIndex = Random.Range(0, Directionss.Length);
            // Vector3 RotationAxis = Directionss[RandomIndex];
            // dice.Rotate(RotationAxis,rotationAngle*Time.deltaTime);

            // dice.
        }
    }
    #endregion

    #region Utilily

    public void HighlightMap()
    {
        
    }

    #endregion
}