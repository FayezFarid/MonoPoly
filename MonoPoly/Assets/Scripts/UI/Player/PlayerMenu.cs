using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenu : MenuBase
{
    //TODO:Get this from scriptable object
    [SerializeField] private Color PlayerTurnColor;
    [SerializeField] private Color DefaultColor;
    
    [SerializeField] private PlayerInfo playerInfo_Prefab;
    private Dictionary<Player,PlayerInfo> _playerInfos=new Dictionary<Player, PlayerInfo>();

    public void InitializeWithPlayers(List<Player> players)
    {
        PlayerInfo playerInfo;
        foreach (var item in players)
        {
            if (_playerInfos.ContainsKey(item))
            {
                SpicyHarissaLogger.LogErrorCritical($"Player [{item.PlayerName}] already exist but we are trying to Initialize it");
                continue;
            }
            
            GameObject infoGO = Instantiate(playerInfo_Prefab.gameObject,default, default,transform);
            playerInfo = infoGO.GetComponent<PlayerInfo>();
            playerInfo.SetMoney(item.Money);
            playerInfo.SetPlayerName(item.PlayerName);
            playerInfo.AssignOnClick(OpenLandMenu);
            item.MoneyChanged  += (int previous, int current,int modifier) => PlayerMoneyChanged(previous,current,modifier,item);
            
            _playerInfos.Add(item,playerInfo);
            
        }
    }

    private void PlayerMoneyChanged(int previous, int current,int modifier,Player player)
    {
        SpicyHarissaLogger.Log($"{player} Money changed new value {current}");
        _playerInfos[player].SetMoney(current);
    }
    private void OpenLandMenu(PlayerInfo playerInfo)
    {
        
    }
    public void SetActivePlayerTurn(Player currentPlayerTurn,Player previousPlayerTurn)
    {
        _playerInfos[previousPlayerTurn].SetColor(DefaultColor);
        _playerInfos[currentPlayerTurn].SetColor(PlayerTurnColor);
    }
}
