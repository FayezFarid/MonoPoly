using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum CurrentState
{
    Default,
    RollingDice,
    DiceRolled,
    Upgrading,
    Mortgage,
    Redeem,
    SellHouse,
}

public class GameManager : MonoBehaviour
{
    #region Events

    public Action<Player /*New player*/> OnNextPlayerTurn;
    public Action<CurrentState /* New state*/> OnCurrentStateChanged;

    public static Action<GameManager /* The Game Manager Instance*/> OnGameOver;

    #endregion

    #region Titles

    private Dictionary<LandTypes, List<LandTitleInstance>> _landTitleInstances;

    #endregion


    //For Debugging
    //this suposedly read only.
    [SerializeField] private List<Player> allPlayers;

    #region Manager

    private TileManager _tileManager;
    public TileManager TileManager => _tileManager;
    private TurnManager _turnManager;
    public TurnManager TurnManager => _turnManager;

    private ScreenMode _screenMode;
    public ScreenMode ScreenMode => _screenMode;

    [SerializeField] private DiceManager _diceManager;

    #endregion

    #region Editor Variables

    //prefabs
    [Header("Scene variables")] [SerializeField]
    private PlayerMenu playerMenu;

    [SerializeField] private BuyLandMenu buyLandMenu;

    [SerializeField] private TradeMenu _tradeMenu;

    [SerializeField] private LandInformationMenu sceneLandInformationMenu;
    [SerializeField] private AuctionMenu auctionMenu;
    [SerializeField] private PrisonMenu prisonMenu;

    [SerializeField] private MatchSettings matchSettings;
    [Header("Dice")] [SerializeField] private Dice dice;
#if UNITY_EDITOR
    public Dice Dice => dice;
#endif
    [SerializeField] private Rigidbody diceRigibody;
    [SerializeField] private int SpinDuration = 5;

    #endregion

    private CurrentState _currentState;

    public CurrentState CurrentState
    {
        get => _currentState;
        private set
        {
            _currentState = value;
            OnCurrentStateChanged?.Invoke(_currentState);
        }
    }

    public Player CurrentPlayer => _turnManager.CurrentPlayerTurn;
    private List<TileLand> _cachedUpgradableTiles;
    private List<TileLand> _cachedSellableLands;
    [SerializeField] private Vector3 _lastSavedDirect;

    private void Start()
    {
        Tile.OnTileClicked += OnLandClicked;
        PrisonStatusEffectDefinition.CurrentPlayerInPrison += CurrentPlayerInPrison;

        _tileManager = GetComponent<TileManager>();
        _tileManager.InitalizeWithGameManager(this);
        _tileManager.SortTitles();

        _screenMode = GetComponent<ScreenMode>();

        _diceManager.Init(matchSettings.NumberOfDices, OnDiceRolled);

        InitPlayers();

        foreach (Player playerItem in allPlayers)
        {
            PlacePlayer(playerItem, 39);
        }

        _turnManager.CurrentPlayerTurn.PlayerTurnStarted();

        StartCoroutine(NextFrameStart());
    }


    private IEnumerator NextFrameStart()
    {
        yield return null;
        OnNextPlayerTurn?.Invoke(CurrentPlayer);
    }

    public void OnLandClicked(Tile tile)
    {
        SpicyHarissaLogger.Log(" GameManager::Tile was clicked", LogLevel.Verbose);
        if (tile is not TileLand)
        {
            return;
        }

        TileLand _tileLand = (TileLand)tile;
        if (_currentState == CurrentState.Upgrading)
        {
            if (_cachedUpgradableTiles.Contains(_tileLand))
            {
                SpicyHarissaLogger.Log(" GameManager::Tile was clicked Condition Met can upgrade",
                    LogLevel.Verbose);
                _turnManager.CurrentPlayerTurn.UpgradePlayerLand(_tileLand.TileInstance);
                // _tileLand.TileInstance.UpgradeLand();
            }
        }
        else if (_currentState == CurrentState.Mortgage)
        {
            if (!_tileLand.LandTitleInstance.IsMortgaged &&
                _turnManager.CurrentPlayerTurn.OwnedLands.Contains(_tileLand.LandTitleInstance))
            {
                SpicyHarissaLogger.Log(" GameManager::Tile was clicked Condition Met Can Mortgage",
                    LogLevel.Verbose);
                // _tileLand.TileInstance.UpgradeLand();
                _turnManager.CurrentPlayerTurn.MortgagePlayerLand(_tileLand.LandTitleInstance);
            }
        }
        else if (_currentState == CurrentState.Redeem)
        {
            if (_tileLand.LandTitleInstance.IsMortgaged &&
                _turnManager.CurrentPlayerTurn.OwnedLands.Contains(_tileLand.LandTitleInstance))
            {
                SpicyHarissaLogger.Log(" GameManager::Tile was clicked Condition Met Can Redeem", LogLevel.Verbose);
                // _tileLand.TileInstance.UpgradeLand();
                _turnManager.CurrentPlayerTurn.RedeemPlayerLand(_tileLand.LandTitleInstance);
            }
        }
        else if (_currentState == CurrentState.SellHouse)
        {
            SpicyHarissaLogger.Log(" GameManager::Tile was clicked Condition Can Sell house", LogLevel.Verbose);
            if (_cachedUpgradableTiles.Contains(_tileLand))
            {
                CurrentPlayer.SellHouse(_tileLand.LandTitleInstance);
            }
        }
        else
        {
            sceneLandInformationMenu.Init(new InitalizationHandle<LandTitleInstance>(_tileLand.LandTitleInstance));
        }
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

        _turnManager = new TurnManager(allPlayers, this);
        playerMenu.InitializeWithPlayers(allPlayers, this);
    }

    private Player CreatePlayer()
    {
        var gameobj = Instantiate(matchSettings.PlayerPrefab);
        return gameobj.GetComponent<Player>();
    }

    #endregion

    #region Auction

    public void OpenAuctionMenu(LandTitleInstance landTitleInstance)
    {
        Debug.Log($"Start auction clicked");
        AuctionMenu.AuctionMenuInitHandle handle = new AuctionMenu.AuctionMenuInitHandle();
        handle.ActivePlayers = _turnManager.ActivePlayers;
        handle.TitleInstance = landTitleInstance;
        handle.OnAuctionEnded = AuctionEnded;
        auctionMenu.Init(new InitalizationHandle<AuctionMenu.AuctionMenuInitHandle>(handle));
    }

    private void AuctionEnded(Player player, LandTitleInstance landTitleInstance, int Cost)
    {
        player.ReduceMoney(Cost);
        player.GainLand(landTitleInstance);
    }

    #endregion

    #region Sell

    public List<TileLand> GetPlayerSellableLands(Player player)
    {
        List<TileLand> tiles = new List<TileLand>();
        //NO one will know this code exist (main thread does tho :( )
        foreach (var KeyPair in player.OwnedLandWithSameType)
        {
            foreach (var land in KeyPair.Value)
            {
                if (land.HasHouses)
                {
                    tiles.Add(land.TileLand);
                }
            }
        }

        return tiles;
    }

    public bool PlayerHaveSellableLands(Player player)
    {
        foreach (var KeyPair in player.OwnedLandWithSameType)
        {
            foreach (var land in KeyPair.Value)
            {
                if (land.HasHouses)
                {
                    return true;
                }
            }
        }

        return false;
    }
    public void ChangeToSellHouses()
    {
        if (_currentState == CurrentState.SellHouse)
        {
            CurrentState = CurrentState.Default;
            ScreenMode.ExitMode();
            return;
        }
        CurrentState = CurrentState.SellHouse;
        _cachedSellableLands = GetPlayerSellableLands(CurrentPlayer);
        
        _screenMode.ChangeToSellMode(_cachedSellableLands);
    }

    #endregion

    #region Trade

    public void OpenTradeMenu(Player OtherPlayer)
    {
        _tradeMenu.EnableMenu(_turnManager.CurrentPlayerTurn, OtherPlayer, this);
    }

    public void DealFinalized(Player player1, TradeInfo tradeInfo1, Player player2, TradeInfo tradeInfo2)
    {
        if (tradeInfo1.Money > 0)
        {
            player1.PayPlayer(player2, tradeInfo1.Money);
        }

        if (tradeInfo2.Money > 0)
        {
            player2.PayPlayer(player1, tradeInfo2.Money);
        }

        foreach (var land in tradeInfo1.TileLands)
        {
            player1.PlayerLosesLand(land);
            player2.GainLand(land);
        }

        foreach (var land in tradeInfo2.TileLands)
        {
            player2.PlayerLosesLand(land);
            player1.GainLand(land);
        }
    }

    #endregion

    #region Redeem

    public void ChangeToRedeemMode()
    {
        
        if (_currentState == CurrentState.Redeem)
        {
            CurrentState = CurrentState.Default;
            ScreenMode.ExitMode();
            return;
        }
        CurrentState = CurrentState.Redeem;

        _screenMode.ChangeToRedeemMode(_turnManager.CurrentPlayerTurn.OwnedLands);
    }

    #endregion

    #region Mortgage

    public void ChangeToMortgageMode()
    {
        if (_currentState == CurrentState.Mortgage)
        {
            CurrentState = CurrentState.Default;
            ScreenMode.ExitMode();
            return;
        }
        CurrentState = CurrentState.Mortgage;

        _screenMode.ChangeToMortgageMode(_turnManager.CurrentPlayerTurn.OwnedLands);
    }

    #endregion

    #region LandUpgrade

    public bool PlayerOwnsLandSet(Player player)
    {
        foreach (var land in player.OwnedLandWithSameType)
        {
            if (_tileManager.LandTypePairedCount[land.Key] == land.Value.Count)
            {
                return true;
            }
        }

        return false;
    }

    public List<TileLand> GetPlayerOwnedLandInstanceSet(Player player)
    {
        List<TileLand> tiles = new List<TileLand>();
        //NO one will know this code exist (main thread does tho :( )
        foreach (var land in player.OwnedLandWithSameType)
        {
            if (_tileManager.LandTypePairedCount[land.Key] == land.Value.Count)
            {
                foreach (var VARIABLE in land.Value)
                {
                    tiles.Add(VARIABLE.TileLand);
                }
            }
        }

        return tiles;
    }

    public void ChangeToUpgradeMode()
    {
        // if(!PlayerOwnsLandSet(_turnManager.CurrentPlayerTurn))
        //     return;
        if (_currentState == CurrentState.Upgrading)
        {
            CurrentState = CurrentState.Default;
            ScreenMode.ExitMode();
            return;
        }
        CurrentState = CurrentState.Upgrading;

        _cachedUpgradableTiles = GetPlayerOwnedLandInstanceSet(CurrentPlayer);
        _screenMode.ChangeToUpgradeLandMode(_cachedUpgradableTiles);
    }

    public void GoBackNormal()
    {
        if (CurrentState == CurrentState.Default)
        {
            ScreenMode.ChangeCameraView();
        }
        else
        {
            CurrentState = CurrentState.Default;
            ScreenMode.ExitMode();
        }
    }

    #endregion

    #region Turn managment

    public void NextPlayerturn()
    {
        CurrentState = CurrentState.Default;
        if (CurrentPlayer.OutOfMoney)
        {
            CurrentPlayer.PlayerForfeit();
            CheckForGameOver();
        }

        _turnManager.CurrentPlayerTurn.PlayerTurnEnded();
        _turnManager.NextTurn();
        _turnManager.CurrentPlayerTurn.PlayerTurnStarted();
    }

    private void CheckForGameOver()
    {
        int numberOfPlayerLost = 0;

        foreach (Player player in _turnManager.AllPlayers)
        {
            numberOfPlayerLost += player.HasLost ? 1 : 0;
        }

        if (_turnManager.PlayersCount - numberOfPlayerLost <= 1)
        {
            SpicyHarissaLogger.Log($"Game over ", LogLevel.Standard, Color.blue);
            OnGameOver?.Invoke(this);
        }
    }

    #endregion

    #region Prison

    private void CurrentPlayerInPrison(StatusEffectInstance prisonInstance)
    {
        PrisonMenu.InitPrisonHandle prisonHandle = new PrisonMenu.InitPrisonHandle();
        prisonHandle.CanPayForPrison = !CurrentPlayer.CanNotAfford(matchSettings.PrisonFee);
        //TODO: Add Card.
        prisonHandle.CanUseCard = false;
        prisonMenu.Init(new InitalizationHandle<PrisonMenu.InitPrisonHandle>(prisonHandle));
    }

    public void CurrentPlayerGoesPrison(StatusEffectDefinition effectDefinition)
    {
        SpicyHarissaLogger.Log($"Going To Prison", LogLevel.Verbose);
        //TODO: ADD revere movement
        StartCoroutine(PlacePlayerAnimated(CurrentPlayer, TileManager.PrisonTile,
            TileManager.GoPrisonTile - TileManager.PrisonTile, true));
        CurrentPlayer.ApplyEffectToSelf(effectDefinition);
    }

    public void CurrentPaysPrisonFee()
    {
        CurrentPlayer.ReduceMoney(matchSettings.PrisonFee);
        CurrentPlayer.RemoveEffectByType(EffectType.Prison);
        prisonMenu.CloseWindow();
    }

    public void PlayerUsesCard()
    {
        CurrentPlayer.RemoveEffectByType(EffectType.PrisonPass);
        CurrentPlayer.RemoveEffectByType(EffectType.Prison);
        prisonMenu.CloseWindow();
    }

    public void PlayerRollForPrison()
    {
        // RollDice()   
    }

    #endregion

    #region Player landed ...

    public void PlayerLandedOnEvent(Player player, EventTileDefinition eventTileDefinition)
    {
        EventDefinition eventDefinition = eventTileDefinition.GetEvent();
        if (eventDefinition == null)
            return;
        StartCoroutine(CreateEvent(eventDefinition));
    }

    //TODO: STOP everything else while this is occuring
    private IEnumerator CreateEvent(EventDefinition eventDefinition)
    {
        ScreenMode.InitEventDescription(eventDefinition.Description);
        yield return new WaitForSeconds(MatchSettings.SingletonInstance.DelayToCloseEventDescription);
        ScreenMode.CloseEventDescription();
        List<Action> actions = eventDefinition.GetActions(this);
        if (actions.Count == 0)
        {
            SpicyHarissaLogger.LogError($"Event Definiton name [{eventDefinition.name} Actions are empty]");
        }

        foreach (var item in actions)
        {
            item.Invoke();
        }
    }

    public void PlayerLandedOnStation(Player player, LandTitleInstance station)
    {
        //TODO:check station shid
        PlayerLandedOnLand(player, station);
    }

    public void PlayerLandedOnLand(Player player, LandTitleInstance landTitleInstance)
    {
        if (landTitleInstance.IsOwned)
        {
            //pay rent 
            PlayerPayRent(player, landTitleInstance);
        }
        else
        {
            //buy
            PlayerBuyLand(player, landTitleInstance);
        }
    }

    public void PlayerBuyLand(Player player, LandTitleInstance landTitleInstance)
    {
        UnityAction<Player, LandTitleInstance> buyaction =
            player.CanNotAfford(landTitleInstance.LandDef.LandValue) ? null : buyLandAction;
        //Todo:Add auction.
        buyLandMenu.EnableMenu(buyaction, (() => OpenAuctionMenu(landTitleInstance)), landTitleInstance, player);
    }

    public void PlayerPayRent(Player player, LandTitleInstance landTitleInstance)
    {
        if (!player.PlayerOwnsLand(landTitleInstance))
            player.PayPlayer(landTitleInstance.Owner, landTitleInstance.GetRent);
    }


    private void buyLandAction(Player player, LandTitleInstance landTitleInstance)
    {
        player.BuyLand(landTitleInstance);
    }

    private void CurrentPlayerDidWholeLoop()
    {
        if (_turnManager.CurrentPlayerTurn.IsFirstRoll)
        {
            _turnManager.CurrentPlayerTurn.IsFirstRoll = false;
            return;
        }

        _turnManager.CurrentPlayerTurn.IncreaseMoney(matchSettings.WholeLoopMoney);
    }

    #endregion

    #region Board movments

    private void OnDiceRolled(int totalValue)
    {
        SpicyHarissaLogger.Log($"Dice Value [{totalValue}] ", LogLevel.Standard);
        //Gotta return back to 0 
        int PositionToPlace = GetNextAvaliableTitle(_turnManager.CurrentPlayerTurn.CurrentPosition, totalValue,
            _tileManager.TileCount);
        if (PositionToPlace < _turnManager.CurrentPlayerTurn.CurrentPosition)
        {
            CurrentPlayerDidWholeLoop();
        }

        StartCoroutine(PlacePlayerAnimated(_turnManager.CurrentPlayerTurn, PositionToPlace, totalValue));
    }

    /// <summary>
    /// Moves current Player . rolls dice etc
    /// </summary>
    public void MoveCurrentPlayer()
    {
        _diceManager.RollDice();
    }


    public int RollDice()
    {
        return dice.Roll();
    }

    public IEnumerator PlacePlayerAnimated(Player PlayerToPlace, int TargetPosition, int MovementsSteps,
        bool inReverse = false)
    {
        CurrentState = CurrentState.RollingDice;
        int startingPosition = PlayerToPlace.CurrentPosition;
        SpicyHarissaLogger.Log(
            $"PlacePlayerAnimated  previousPosition [{startingPosition}] TargetPosition [{TargetPosition}]",
            LogLevel.Verbose);
        // WaitForSeconds waitForSeconds = new WaitForSeconds(matchSettings.DurationBetweenPlayerPlacement);
        int currentPosition = startingPosition;
        Vector3 previousDirection = _lastSavedDirect;
        float speed = matchSettings.PlayerMovementSpeed;
        for (int i = 1; i <= MovementsSteps; i++)
        {
            //TODO: Look at next tile.
            if (inReverse)
            {
                currentPosition = GetPreviousAvaliableTile(currentPosition, 1, _tileManager.TileCount);
            }
            else
            {
                currentPosition = GetNextAvaliableTitle(currentPosition, 1, _tileManager.TileCount);
            }

            Tile tile = _tileManager[currentPosition];

            yield return PlayerRotate(PlayerToPlace.transform, tile.transform, previousDirection);

            previousDirection = PlayerToPlace.transform.position.GetDirection(tile.transform.position);


            // _tileManager[i].PlacePlayer(PlayerToPlace.transform);
            while (Vector3.Distance(PlayerToPlace.transform.position, tile.transform.position) >
                   Time.deltaTime)
            {
                var step = speed * Time.deltaTime;
                PlayerToPlace.transform.position = Vector3.MoveTowards(PlayerToPlace.transform.position,
                    tile.transform.position, step);

                yield return new WaitForSeconds(Time.deltaTime);
            }

            SpicyHarissaLogger.Log(
                $"PlacePlayerAnimated Loop ended  i [{i}] Tile[{tile.PositionInMap}] ]",
                LogLevel.Verbose);
        }

        _lastSavedDirect = previousDirection;
        yield return new WaitForSeconds(Time.deltaTime);

        PlacePlayer(PlayerToPlace, TargetPosition);
    }


    private IEnumerator PlayerRotate(Transform playerTransform, Transform tileTransform, Vector3 previousDirection)
    {
        float Angle = Quaternion.Angle(playerTransform.rotation, tileTransform.rotation);
        var heading = tileTransform.position - playerTransform.position;
        var distance = heading.magnitude;
        var direction = heading / distance; // This is now the normalized direction.
        Debug.Log($"Angle {Angle}  direction = {direction} PreviousDirection = {previousDirection} ");

        // yield return null;

        // if (direction == previousDirection  || previousDirection==Vector3.zero)
        // {
        yield break;

        // }
        // Debug.Log($"Angle Are different starting rotation");
        // float speed = matchSettings.PlayerMovementSpeed;
        //
        // var step = speed * Time.deltaTime;
        // currentVelocity = 0;
        // float currentAngle = step;
        // playerTransform.transform.rotation=Quaternion.identity;
        // while (playerTransform.eulerAngles.y > Angle)
        // {
        //      currentAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y , Angle ,ref currentVelocity ,step );
        //     playerTransform.Rotate(Vector3.up,currentAngle );
        //     // currentAngle += step;
        //     // currentAngle=  Mathf.Clamp(currentAngle, 0, Angle);
        //     yield return new WaitForSeconds(1);
        //
        // }

        // yield return new WaitForSeconds(10);
    }

    public void PlacePlayer(Player PlayerToPlace, int position)
    {
        SpicyHarissaLogger.Log(
            $"PlacePlayer Standard  Player To place [{PlayerToPlace}] TargetPosition [{position}]",
            LogLevel.Verbose);

        //divided whatever visual and triggering effect of where player landed
        _tileManager[position].PlacePlayer(PlayerToPlace.transform);
        PlayerToPlace.CurrentPosition = position;
        _tileManager.PlayerLandedOnTile(PlayerToPlace, position);


        int NextTile = GetNextAvaliableTitle(position, 1, _tileManager.TileCount);
        PlayerToPlace.transform.LookAt(_tileManager[NextTile].transform);

        CurrentState = CurrentState.DiceRolled;
    }

    #endregion

    #region Utilily

    public static int GetNextAvaliableTitle(int CurrentPosition, int DiceValue, int TotalLength)
    {
        int returnValue = CurrentPosition;
        TotalLength -= 1;
        for (int i = 0; i < DiceValue; i++)
        {
            returnValue = Extension.GetNextIndex(returnValue, TotalLength);
        }

        return returnValue;
    }

    public static int GetPreviousAvaliableTile(int CurrentPosition, int DiceValue, int TotalLength)
    {
        int returnValue = CurrentPosition;
        TotalLength -= 1;
        for (int i = 0; i < DiceValue; i++)
        {
            if (returnValue == 0)
            {
                returnValue = TotalLength;
            }

            returnValue--;
        }

        return returnValue;
    }

    #endregion
}