using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMenu : MenuBase
{
    //TODO:Get this from scriptable object
  
    
    [SerializeField] private PlayerInfo playerInfo_Prefab;
    
    private GameManager _gameManager;
    private Dictionary<Player,PlayerInfo> _playerInfos=new Dictionary<Player, PlayerInfo>();

    public void InitializeWithPlayers(List<Player> players,GameManager gameManager)
    {
        PrisonStatusEffectDefinition.CurrentPlayerLeftPrison += CurrentPlayerLeftPrison;
        PrisonStatusEffectDefinition.CurrentPlayerInPrison += CurrentPlayerInPrison;
        Player.OnPlayerLost+= OnPlayerLost;
        _gameManager = gameManager;
        gameManager.OnNextPlayerTurn+= OnNextPlayerTurn;
        
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
            
            playerInfo.Init(new InitalizationHandle<Player>(item));
            // playerInfo.SetMoney(item.Money);
            // playerInfo.SetPlayerName(item.PlayerName);
            // playerInfo.AssignOnClick(OpenLandMenu);
            item.MoneyChanged  += (previous, current, modifier) => PlayerMoneyChanged(previous,current,modifier,item);
            
            _playerInfos.Add(item,playerInfo);
            
        }
    }

    private void CurrentPlayerInPrison(StatusEffectInstance obj)
    {
        _playerInfos[_gameManager.CurrentPlayer].SetPrisonState(true);  
    }

    private void CurrentPlayerLeftPrison(StatusEffectInstance obj)
    {
        _playerInfos[_gameManager.CurrentPlayer].SetPrisonState(false);  
    }

    private void OnPlayerLost(Player obj)
    {
        _playerInfos[obj].SetColor(MatchSettings.SingletonInstance.PlayerLostColor);
    }

    private void OnNextPlayerTurn(Player obj)
    {
        SetActivePlayerTurn(obj, _gameManager.TurnManager.PreviousPlayer);
    }

    private void PlayerMoneyChanged(int previous, int current,int modifier,Player player)
    {
        SpicyHarissaLogger.Log($"{player} Money changed Modifier {modifier}",LogLevel.Standard);
        _playerInfos[player].SetMoney(current);
        CreateMoneyCardPopup(_playerInfos[player].transform as RectTransform,modifier);
    }
    
    private void CreateMoneyCardPopup(RectTransform targetTransform,int MoneyValue)
    {
        //TODO: make this pool
        Transform canvas = GameObject.Find("Canvas").transform;
        var instantedgameObject = Instantiate(MatchSettings.SingletonInstance.MoneyCardPopUpPrefab,default,default,canvas);
        
        //Common position with canvas
       
        // Vector2 newPosition = targetTransform.localPosition;
        Vector2 newPosition= canvas.InverseTransformPoint(targetTransform.position);
        newPosition.x -= targetTransform.rect.width / 2 + 50;
        instantedgameObject.transform.localPosition = newPosition;

        instantedgameObject.Init(new InitalizationHandle<int>(MoneyValue));
        StartCoroutine(closePopupAfterDelay(instantedgameObject.gameObject));
    }

    private IEnumerator closePopupAfterDelay(GameObject target)
    {
        //TODO: make this pool
        yield return new WaitForSeconds(MatchSettings.SingletonInstance.DelayMoneyPopup);
        // target.SetActive(false);
        Destroy(target);
    }
    private void OpenLandMenu(PlayerInfo playerInfo)
    {
        //TODO: Implement this
    }
    private void SetActivePlayerTurn(Player currentPlayerTurn,Player previousPlayerTurn)
    {
      
        // _gameManager.
        if(!previousPlayerTurn.HasLost)
            _playerInfos[previousPlayerTurn].SetColor(MatchSettings.SingletonInstance.DefaultColor);
        _playerInfos[currentPlayerTurn].SetColor(MatchSettings.SingletonInstance.PlayerTurnColor);
    }

    public override void Init<T>(InitalizationHandle<T> handle)
    {
        throw new System.NotImplementedException();
    }

    public override void CloseWindow()
    {
        throw new System.NotImplementedException();
    }
}
