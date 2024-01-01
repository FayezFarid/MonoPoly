using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


[System.Serializable]
public struct TradeSideFields
{
    public Multi_InteractionButton confirmButton;
    public Transform landPanel;
    public TMP_InputField moneyInputField;

    public SmallPlayerInfoCard SmallPlayerInfo;
}

public struct TradeInfo
{
    public TradeInfo(int initalMoney)
    {
        TileLands = new List<LandTitleInstance>();
        Money = initalMoney;
    }

    public List<LandTitleInstance> TileLands;
    public int Money;
}

public class TradeMenu : MenuBase
{
    private Player _player1;
    private Player _player2;

    private bool _player1Ready;
    private bool _player2Ready;

    private Dictionary<Player, List<SmallLandCard>> playerLandCardRelation;
    private Dictionary<Player, TradeInfo> _tradeInfoMap;

    private GameManager _gameManager;

    #region Unity Fields

    [FormerlySerializedAs("tradeSideRight")] [SerializeField]
    private TradeSideFields tradeSideFieldsRight;

    [FormerlySerializedAs("tradeSideLeft")] [SerializeField]
    private TradeSideFields tradeSideFieldsLeft;

    [SerializeField] private Button finalizeDealButton;
    [SerializeField] private Button dockButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject DealIsDone;

    [Header("Prefabs")] [SerializeField] private SmallLandCard smallLandCardPrefab;

    #endregion


    public void EnableMenu(Player player1, Player player2, GameManager gameManager)
    {
        _gameManager = gameManager;
        _player1 = player1;
        _player2 = player2;
        playerLandCardRelation = new Dictionary<Player, List<SmallLandCard>>();
        _tradeInfoMap = new Dictionary<Player, TradeInfo>();
        _tradeInfoMap.Add(player1, new TradeInfo(0));
        _tradeInfoMap.Add(player2, new TradeInfo(0));
        //TODO: MultiInteraction Button init.
        SetupTradeSide(tradeSideFieldsLeft, player1, true);
        SetupTradeSide(tradeSideFieldsRight, player2, false);
        gameObject.SetActive(true);
    }

    private void SetupTradeSide(TradeSideFields sideFields, Player player, bool PlayerLeftSide)
    {
        //TODO: use existing ones?
        if (player == null)
            return;
        // Lands
        List<SmallLandCard> cards = new List<SmallLandCard>();
        playerLandCardRelation.Add(player, cards);
        GameObject cardGO = null;
        SmallLandCard.SmallLandCardInitHandle smallLandCardInitHandle = new SmallLandCard.SmallLandCardInitHandle();
        SmallPlayerInfoCard.SmallPlayerInfoCardHandle smallPlayerInfoCardHandle =
            new SmallPlayerInfoCard.SmallPlayerInfoCardHandle();
        foreach (var land in player.OwnedLands)
        {
            cardGO = Instantiate(smallLandCardPrefab.gameObject, default, default, sideFields.landPanel);
            SmallLandCard landCard = cardGO.GetComponent<SmallLandCard>();


            smallLandCardInitHandle.Land = land;
            smallLandCardInitHandle.OnClickedEvent = LandClicked;
            smallLandCardInitHandle.DirectionToLeft = PlayerLeftSide;

            landCard.Init(new InitalizationHandle<SmallLandCard.SmallLandCardInitHandle>(smallLandCardInitHandle));
            // landCard.Init(smallLandCardInitHandle);
            cards.Add(landCard);
        }


        smallPlayerInfoCardHandle.playerName = player.PlayerName;
        smallPlayerInfoCardHandle.playerColor = player.PlayerColor;
        //Player 
        sideFields.SmallPlayerInfo.Init(
            new InitalizationHandle<SmallPlayerInfoCard.SmallPlayerInfoCardHandle>(smallPlayerInfoCardHandle));


        //Confirm buttons.
        sideFields.confirmButton.Add_SingleAction_Green_Switchable(() => ConfirmAction(PlayerLeftSide));
    }

    private void LandClicked(SmallLandCard clickedCard, LandTitleInstance landTitleInstance)
    {
        Player target = null;
        foreach (var item in playerLandCardRelation)
        {
            if (item.Value.Contains(clickedCard))
            {
                target = item.Key;
                break;
            }
        }

        if (!target)
        {
            SpicyHarissaLogger.LogErrorCritical(
                "TradeMenu::LandClicked land was clicked that was not associated with any player");
            return;
        }

        if (_tradeInfoMap[target].TileLands.Contains(landTitleInstance))
        {
            _tradeInfoMap[target].TileLands.Remove(landTitleInstance);
        }
        else
        {
            _tradeInfoMap[target].TileLands.Add(landTitleInstance);
        }
    }


    private void ConfirmAction(bool Player1 /*if not player one it's player 2*/)
    {
        if (Player1)
        {
            _player1Ready = !_player1Ready;
        }
        else
        {
            _player2Ready = !_player2Ready;
        }

        CheckIfCanFinalizeDeal();
    }

    private void CheckIfCanFinalizeDeal()
    {
        finalizeDealButton.interactable = _player1Ready && _player2Ready;
    }

    #region Unity Event seralized

    public void FinializeDeal()
    {
        int leftMoney = 0;
        int rightMoney = 0;
        if (!string.IsNullOrEmpty(tradeSideFieldsLeft.moneyInputField.text))
            leftMoney = Int32.Parse(tradeSideFieldsLeft.moneyInputField.text);
        if (!string.IsNullOrEmpty(tradeSideFieldsRight.moneyInputField.text))
            rightMoney = Int32.Parse(tradeSideFieldsRight.moneyInputField.text);

        //Left
        TradeInfo tradeInfo1 = _tradeInfoMap[_player1];
        tradeInfo1.Money = leftMoney;
        // _tradeInfoMap[_player1] = tradeInfo1;

        //Right 
        TradeInfo tradeInfo2 = _tradeInfoMap[_player2];
        tradeInfo2.Money = rightMoney;
        // _tradeInfoMap[_player2] = tradeInfo2;

        //TODO: call gamemanger add it in pass in
        _gameManager.DealFinalized(_player1, tradeInfo1, _player2, tradeInfo2);
        CloseAfterDealDone();
    }

    public override void Init<T>(InitalizationHandle<T> handle)
    {
        throw new NotImplementedException();
    }

    public override void CloseWindow()
    {
        ResetWindow();
        gameObject.SetActive(false);
    }

    public void CloseAfterDealDone()
    {
        ResetWindow();
        StartCoroutine(CloseDealAfterDelay());
    }


    protected override void ResetWindow()
    {
        foreach (var item in playerLandCardRelation)
        {
            foreach (var VARIABLE in item.Value)
            {
                Destroy(VARIABLE.gameObject);
            }
        }

        playerLandCardRelation = new Dictionary<Player, List<SmallLandCard>>();
        _tradeInfoMap = new Dictionary<Player, TradeInfo>();
        _player1 = null;
        _player2 = null;
        _player1Ready = false;
        _player2Ready = false;
    }

    private IEnumerator CloseDealAfterDelay()
    {
        DealIsDone.SetActive(true);
        yield return new WaitForSeconds(MatchSettings.SingletonInstance.DelayAfterClosingMenu);
        DealIsDone.SetActive(false);
        gameObject.SetActive(false);
    }

    #endregion
}