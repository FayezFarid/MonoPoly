using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
    private List<Player> _players;
    private GameManager _gameManager;

    private Index _currentPlayerIndex;
    
    
    public IEnumerable<Player> AllPlayers => _players;

    public List<Player> ActivePlayers
    {
        get
        {
            //TODO: preallocate
            List<Player> players = new List<Player>();
            foreach (var VARIABLE in _players)
            {
                if(!VARIABLE.HasLost)
                    players.Add(VARIABLE);
            }

            return players;
        }
    }

    public Player CurrentPlayerTurn => _players[_currentPlayerIndex.CurrentIndex];
    public Player PreviousPlayer => GetPreviousPlayer(GetLastIndex(_currentPlayerIndex.CurrentIndex));
    public int PlayersCount => _players.Count;

    
    public TurnManager(List<Player> players,GameManager gameManager)
    {
        this._players = players;
        _gameManager = gameManager;
        _currentPlayerIndex = new Index(0, players.Count, true);
    }
    private Player GetPreviousPlayer(int StartIndex)
    {
        if (_players[StartIndex].HasLost)
        {
            return GetPreviousPlayer(GetLastIndex(StartIndex));
        }

        return _players[StartIndex];
    }
    public void NextTurn(int depth=0)
    {
        // if (_currentPlayerIndex == _players.Count - 1)
        // {
        //     _currentPlayerIndex = 0;
        // }
        // else
        // {
        //     _currentPlayerIndex++;
        // }
        _currentPlayerIndex.MoveNextIndex();
        if (CurrentPlayerTurn.HasLost)
        {
            if (depth >= _players.Count - 1)
            {
                SpicyHarissaLogger.LogErrorCritical("player depth has became superior  than the players count");
                return;
            }
            NextTurn(depth++);
            return;
        }
        _gameManager.OnNextPlayerTurn?.Invoke(CurrentPlayerTurn);
    }


    public int GetLastIndex(int currentIndex)
    {
        if (currentIndex == 0)
        {
            return _players.Count - 1;
        }

        return currentIndex - 1;
    }
}
