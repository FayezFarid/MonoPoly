using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
    private List<Player> _players;

    private int _currentPlayerIndex;
    
    public IEnumerable<Player> AllPlayers => _players;

    public Player CurrentPlayerTurn => _players[_currentPlayerIndex];
    public Player PreviousPlayer => _players[GetLastIndex()];

    public void NextTurn()
    {
        if (_currentPlayerIndex == _players.Count - 1)
        {
            _currentPlayerIndex = 0;
        }
        else
        {
            _currentPlayerIndex++;
        }
        
    }

    public TurnManager(List<Player> players)
    {
        this._players = players;
    }

    public int GetLastIndex()
    {
        if (_currentPlayerIndex == 0)
        {
            return _players.Count - 1;
        }

        return _currentPlayerIndex - 1;
    }
}
