using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AuctionMenu : MenuBase
{
    public static string PREVIOUS_BID = "PreviousBid : ";
    
    
    [SerializeField] private Transform biddingPlayersZone;
    [SerializeField] private TMP_InputField moneyInput;
    [SerializeField] private TextMeshProUGUI previousBid;

    [SerializeField] private GameObject auctionEndedPanel;
    [SerializeField] private TextMeshProUGUI playerWonText;
    [SerializeField] private LandInformationMenu _landInformationMenu;

    // [SerializeField] private Button nextBid;


    public struct AuctionMenuInitHandle
    {
        public LandTitleInstance TitleInstance;
        public List<Player> ActivePlayers;
        public Action<Player /*WinningPlayer*/, LandTitleInstance, int /*Price*/> OnAuctionEnded;
    }


    private AuctionMenuInitHandle _initHandle;

    //this is almost becoming sub system it self
    private Index currentPlayerIndex;

    private Dictionary<Player, SmallPlayerInfoCard> _playerInfoCardsMap = new Dictionary<Player, SmallPlayerInfoCard>();

    private int _currentBid;
    private int _inputBid;

    public int MiniumNewBid
    {
        get
        {
            float rawValue = _currentBid + (_currentBid * MatchSettings.SingletonInstance.BidIncreasePercentage);
            // + MatchSettings.SingletonInstance.StaticIncreaseAmount;
            return Mathf.CeilToInt(rawValue);
        }
    }

    public override void Init<T>(InitalizationHandle<T> handle)
    {
        _initHandle = (AuctionMenuInitHandle)(handle.Data as object);
        currentPlayerIndex = new Index(0, _initHandle.ActivePlayers.Count, true);
        SmallPlayerInfoCard.SmallPlayerInfoCardHandle initPara = new SmallPlayerInfoCard.SmallPlayerInfoCardHandle();
        foreach (var player in _initHandle.ActivePlayers)
        {
            SmallPlayerInfoCard card = Instantiate(MatchSettings.SingletonInstance.PlayerInfoCard, default, default,
                biddingPlayersZone);
            initPara.Init(player);

            card.Init(new InitalizationHandle<SmallPlayerInfoCard.SmallPlayerInfoCardHandle>(initPara));
            _playerInfoCardsMap.Add(player, card);
        }

        _currentBid = 1;
        previousBid.text = PREVIOUS_BID + _currentBid;

        _playerInfoCardsMap[_initHandle.ActivePlayers[currentPlayerIndex.CurrentIndex]]
            .ChangeParentImageColor(Color.green);
        
        
        _landInformationMenu.Init(new InitalizationHandle<LandTitleInstance>(_initHandle.TitleInstance));
        gameObject.SetActive(true);
        
    }


    public override void CloseWindow()
    {
        foreach (var VARIABLE in _playerInfoCardsMap)
        {
            Destroy(VARIABLE.Value.gameObject);
        }

        _playerInfoCardsMap.Clear();
        gameObject.SetActive(false);
    }

    public void OnMoneyInputValueChanged(string newValue)
    {
        _inputBid = int.Parse(newValue);
        if (_inputBid < MiniumNewBid)
        {
            moneyInput.text = MiniumNewBid.ToString();
            _inputBid = MiniumNewBid;
        }
    }

    //Button Assigned
    public void NextPlayer()
    {
        int previousIndex = currentPlayerIndex.GetPrevious;
        int BeforeNext = currentPlayerIndex.CurrentIndex;
        _playerInfoCardsMap[_initHandle.ActivePlayers[previousIndex]]
            .ChangeParentImageColor(Color.gray);
        _playerInfoCardsMap[_initHandle.ActivePlayers[BeforeNext]]
            .ChangeParentImageColor(Color.yellow);

        currentPlayerIndex.MoveNextIndex();

        int AfterNext = currentPlayerIndex.CurrentIndex;
        _playerInfoCardsMap[_initHandle.ActivePlayers[AfterNext]]
            .ChangeParentImageColor(Color.green);

        SpicyHarissaLogger.Log($"_inputBed [{_inputBid}] MiniumNewBid [{MiniumNewBid}] _currentBid [{_currentBid}] ",
            LogLevel.Verbose);
        if (_inputBid < MiniumNewBid)
        {
            _inputBid = MiniumNewBid;
        }

        _currentBid = _inputBid;
        previousBid.text = PREVIOUS_BID + _currentBid;
        _inputBid = 0;

        // SpicyHarissaLogger.Log($"previousIndex [{previousIndex}] BeforeNext [{BeforeNext}] AfterNext [{AfterNext}] ",LogLevel.Verbose);
    }

    //Button Assigned
    public void Fold()
    {
        //Just remove that player no need for extra shit as will allocate more
        Debug.Log($"old Index {currentPlayerIndex.CurrentIndex} ");
        Player player = _initHandle.ActivePlayers[currentPlayerIndex.CurrentIndex];
        _initHandle.ActivePlayers.Remove(player);
        currentPlayerIndex.MaxLength = _initHandle.ActivePlayers.Count;
        Debug.Log($"New Index {currentPlayerIndex.CurrentIndex} ");

        _playerInfoCardsMap[player].ChangeParentImageColor(Color.red);
        _playerInfoCardsMap[_initHandle.ActivePlayers[currentPlayerIndex.CurrentIndex]]
            .ChangeParentImageColor(Color.green);

        if (_initHandle.ActivePlayers.Count == 1)
        {
            OnAuctionEnded();
            // return;
        }
    }

    private void OnAuctionEnded()
    {
        SpicyHarissaLogger.Log(
            $"Auction Ended {_initHandle.ActivePlayers[currentPlayerIndex.CurrentIndex]} Won Auction At {_currentBid}",
            LogLevel.Standard);
        StartCoroutine(CloseDealAfterDelay());

    }
    private IEnumerator CloseDealAfterDelay()
    {
        auctionEndedPanel.SetActive(true);
        yield return new WaitForSeconds(MatchSettings.SingletonInstance.DelayAfterClosingMenu);
        auctionEndedPanel.SetActive(false);
        _initHandle.OnAuctionEnded?.Invoke(_initHandle.ActivePlayers[currentPlayerIndex.CurrentIndex],
            _initHandle.TitleInstance, _currentBid);
        CloseWindow();
    }
}