using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a component with player we consider player as state and this as controller
public class AIController : MonoBehaviour
{
    private bool _active;

    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            if (_active)
                Activate();
            else
                Deactivate();
        }
    }

    public bool FullAI { get; set; }

    public enum State
    {
        Default,
        WaitingToStartTurn,
        PlayerRolled,
        WaitingForPlayerToLand,
        PlayerLanded,
    }

    private float _delayBetweenAction;
    [SerializeField] private Player _player;
    private GameManager _gameManager;
    private BuyLandMenu _buyLandMenu;

    private Coroutine _mainCoroutine;
    private Coroutine _LandedCoroutine;
    private bool _playerCanGoAgain = false;

    private State _currentState;

    private State currentState
    {
        get => _currentState;
        set
        {
            Debug.Log($"Switching to State [{value}]");
            _currentState = value;
        }
    }

    public void Start()
    {
        //lot of find 
        // _player = GetComponent<Player>();
        _buyLandMenu = FindObjectOfType<BuyLandMenu>(true);
        _gameManager = FindObjectOfType<GameManager>();

        _delayBetweenAction = MatchSettings.SingletonInstance.DelayBetweenAction;

        _gameManager.OnPlayerLandedOnTile += OnPlayerLandedOnTile;
        _gameManager.OnCurrentStateChanged += OnCurrentStateChanged;
        _gameManager.OnNextPlayerTurn += OnNextPlayerTurn;
    }

    public void Reset()
    {
        currentState = State.Default;
    }

    private void Activate()
    {
    }

    private void Deactivate()
    {
        if (_mainCoroutine != null)
            StopCoroutine(_mainCoroutine);
        if (_LandedCoroutine != null)
            StopCoroutine(_LandedCoroutine);
    }


    private void OnNextPlayerTurn(Player player)
    {
        if (!Active)
            return;

        SpicyHarissaLogger.Log($"[{_player}] OnNextPlayerTurn, Can Play [{_player == player}]  ", LogLevel.Verbose,
            "AI");

        if (_player != player)
        {
            if (_mainCoroutine != null)
                StopCoroutine(_mainCoroutine);
            return;
        }

        SpicyHarissaLogger.Log(
            $"[{_player}] Going to start MainCoroutine MainCourtine already running [{_mainCoroutine != null}]  ",
            LogLevel.Verbose,
            "AI");
        if (_mainCoroutine != null)
        {
            _playerCanGoAgain = true;
        }
        else
        {
            _mainCoroutine = StartCoroutine(MainCoroutine());
        }
    }

    private void OnPlayerLandedOnTile(Player arg1, Tile arg2)
    {
        if (!Active)
            return;
        if (_player != arg1)
        {
            return;
        }

        SpicyHarissaLogger.Log($"[{arg1}] AI Controller has landed ", LogLevel.Verbose,
            "AI");
        if (_LandedCoroutine != null)
        {
            StopCoroutine(_LandedCoroutine);
        }

        _LandedCoroutine = StartCoroutine(CheckTileLanded(arg1, arg2));
    }

    private IEnumerator MainCoroutine()
    {
        if (!Active)
            yield break;

        _playerCanGoAgain = false;

        currentState = State.WaitingToStartTurn;

        //TODO: check if in prison first
        yield return new WaitForSeconds(_delayBetweenAction);
        SpicyHarissaLogger.Log($"Moving Current Player [{_gameManager.CurrentPlayer.PlayerName}] ]", LogLevel.Verbose,
            "AI");
        //Makes sure it has not been rolled first
        if (_gameManager.CurrentState is not (CurrentState.RollingDice and CurrentState.DiceRolled))
            _gameManager.MoveCurrentPlayer();
        currentState = State.WaitingForPlayerToLand;
        //wait for player to land
        while (currentState != State.PlayerLanded)
        {
            // Debug.Log($"Waiting for state to become landed Current [{currentState}]");
            yield return null;
        }

        yield return new WaitForSeconds(_delayBetweenAction);
        if (!FullAI)
        {
            if (!_playerCanGoAgain)
            {
                SpicyHarissaLogger.Log(
                    $"Switching to Next Player turn. current [{_gameManager.CurrentPlayer.PlayerName}] ]",
                    LogLevel.Verbose,
                    "AI");
                _gameManager.NextPlayerturn();
            }
        }

        _mainCoroutine = _playerCanGoAgain ? StartCoroutine(MainCoroutine()) : null;
    }

    private void OnCurrentStateChanged(CurrentState NewState)
    {
        if (!Active)
            return;
        if (_player != _gameManager.CurrentPlayer)
            return;
        if (NewState == CurrentState.DiceRolled)
        {
        }

        if (!FullAI)
        {
            return;
        }
        //Read if can upgrade etc etc 
    }


    private IEnumerator CheckTileLanded(Player player, Tile tile)
    {
        //todo: whatever the fuck imma write imporve it

        if (tile.TitleType is TitleType.Land or TitleType.Station)
        {
            //means already bought by clicking
            if (_buyLandMenu.gameObject.activeInHierarchy)
            {
                TileLand tileLand = (TileLand)tile;
                LandTileDefinition LandTileDef = (LandTileDefinition)tileLand.TitleDefinition;
                //if can buy,buy it else auction.
                if (_gameManager.CurrentPlayer.CanNotAfford(LandTileDef.LandValue))
                {
                    _gameManager.OpenAuctionMenu(tileLand.LandTitleInstance);
                }
                else
                {
                    _gameManager.CurrentPlayer.BuyLand(tileLand.LandTitleInstance);
                }

                _buyLandMenu.CloseWindow();
            }
        }
        // else if (tile.TitleType == TitleType.Prison)
        // {
        //     PlayerLandedOnPrison(player, tile);
        // }
        else if (tile.TitleType == TitleType.SpecialEvent || tile.TitleType == TitleType.Parking)
        {
            float waitTime = MatchSettings.SingletonInstance.DelayToCloseEventDescription * 0.5f;
            waitTime += MatchSettings.SingletonInstance.DelayToCloseEventDescription;
            yield return new WaitForSeconds(waitTime);
        }

        // else if (tile.TitleType == TitleType.Station)
        // {
        //     PlayerLandedOnStation(player, tile);
        // }
        currentState = State.PlayerLanded;
    }
}