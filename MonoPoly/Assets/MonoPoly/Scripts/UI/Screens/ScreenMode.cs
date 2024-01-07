using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public struct MidScreenRef
{
    public GameObject MidGameObject;
    public TextMeshProUGUI MidText;
}

public class ScreenMode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public const string NEXT_TURN_TEXT = "Next turn";
    public const string BROKE = "Forfeit ";
    public const string UPGRADING = "Upgrading";
    public const string MORTGAGING = "Mortgaging";
    public const string REDEEMING = "Redeeming";
    public const string SELLING = "Selling";
    public const string DEACTIVATE_AI = "Deactivate Auto Play";
    public const string ACTIVATE_AI = "Activate Auto Play";

    #region Internal struct

    public enum CurrentCameraMode
    {
        TopDown,
        Pov,
    }

    [Serializable]
    public struct Mid_BottomScreenRef
    {
        public RectTransform ContainerGameObject;
        public Button nextTurnButton;
        public TextMeshProUGUI nextTurnText;

        public Button rollButton;
    }

    [System.Serializable]
    public struct LeftSideButton
    {
        public Button mortgageButton;
        public Button upgradeButton;
        public Button redeemButton;
        public Button tradeButton;
        public Button sellButton;
    }

    #endregion


    #region Seralized field

    [SerializeField] private Camera mainCamera;

    [Header("Buttons")] [SerializeField] private LeftSideButton leftSideButton;
    [SerializeField] private Mid_BottomScreenRef mid_BottomScreenRef;
    [SerializeField] private MidScreenRef midScreenRef;

    [Header("Trade")] [SerializeField] private Transform tradePlayerParent;
    [SerializeField] private SmallPlayerInfoCard smallPlayerInfoCard_Prefab;

    [SerializeField] private EventDescriptionCard eventDescriptionCard;
    [SerializeField] private TextMeshProUGUI switchAutoPlayButtonText;

    #endregion

    private GameManager _gameManager;
    private Dictionary<Player, SmallPlayerInfoCard> _activePlayerInfos = new Dictionary<Player, SmallPlayerInfoCard>();

    private bool _firstPersonView = true;


    private void Start()
    {
        _firstPersonView = true;
        TileInstance.OnLandOwnerChanged += OnLandOwnerChanged;
        _gameManager = GetComponent<GameManager>();
        _gameManager.OnNextPlayerTurn += OnNexPlayerTurn;
        _gameManager.OnCurrentStateChanged += OnCurrentStateChanged;
        Player.OnPlayerBroke += PlayerBroke;
        Player.OnPlayerEscapedBroke += OnPlayerEscapedBroke;
        mid_BottomScreenRef.nextTurnText = mid_BottomScreenRef.nextTurnButton.GetComponentInChildren<TextMeshProUGUI>();
        StartCoroutine(NextFrameUpdate());
    }

    private IEnumerator NextFrameUpdate()
    {
        yield return null;

        if (MatchSettings.SingletonInstance.UseAppropriateColors)
        {
            HighLightAllToProperColor();
        }
    }


    private void OnLandOwnerChanged(LandTitleInstance titleInstance, Player player)
    {
        if (player == _gameManager.CurrentPlayer)
        {
            HandleAllButtons();
        }
    }

    private void OnCurrentStateChanged(CurrentState obj)
    {
        if (obj == CurrentState.RollingDice)
        {
            // nextTurnButton.gameObject.SetActive(true);
            mid_BottomScreenRef.rollButton.interactable = false;
            HandleAllButtons();
        }

        if (obj == CurrentState.DiceRolled)
        {
            mid_BottomScreenRef.nextTurnButton.gameObject.SetActive(true);
            mid_BottomScreenRef.rollButton.gameObject.SetActive(false);
        }

        bool condition = obj < CurrentState.Upgrading;
        if (condition)
        {
            ExitMode();
            CloseMidScreen();
        }
    }

    private void OnPlayerEscapedBroke(Player obj)
    {
        ExitPlayerBrokeScreen();
    }

    private void PlayerBroke(Player player)
    {
        SetPlayerBrokeScreen();
    }

    private void ExitPlayerBrokeScreen()
    {
        ChangeNextPlayerTurnButtonToDefault();
        // rollButton.gameObject.SetActive(false);
    }

    private void SetPlayerBrokeScreen()
    {
        ChangeNextPlayerTurnButtonToBroke();
        // rollButton.gameObject.SetActive(false);
    }

    private void ChangeNextPlayerTurnButtonToBroke()
    {
        mid_BottomScreenRef.nextTurnButton.image.color = Color.red;
        //cache it ?
        mid_BottomScreenRef.nextTurnText.text = BROKE;
        // mid_BottomScreenRef. nextTurnText.color=Color.red;
    }

    private void ChangeNextPlayerTurnButtonToDefault()
    {
        mid_BottomScreenRef.nextTurnButton.image.color = Color.white;
        //cache it ?
        mid_BottomScreenRef.nextTurnText.text = NEXT_TURN_TEXT;
    }

    private void OnNexPlayerTurn(Player currentPlayer)
    {
        _gameManager.TurnManager.PreviousPlayer.SetCameraActive(false);
        if (_firstPersonView)
        {
            currentPlayer.SetCameraActive(true);
        }

        HandleAllButtons();
        ChangeNextPlayerTurnButtonToDefault();
        mid_BottomScreenRef.rollButton.interactable = true;
        mid_BottomScreenRef.rollButton.gameObject.SetActive(true);
        mid_BottomScreenRef.nextTurnButton.gameObject.SetActive(false);
        _tradeMenuOpen = false;
        ChangeTradeCardsActivity(false);
    }

    #region Event Description

    public void InitEventDescription(string description)
    {
        EventDescriptionCard.EventDescriptionCardInitHandle handle =
            new EventDescriptionCard.EventDescriptionCardInitHandle()
            {
                Description = description
            };
        eventDescriptionCard.Init(new InitalizationHandle<EventDescriptionCard.EventDescriptionCardInitHandle>(handle));
    }

    public void CloseEventDescription()
    {
        eventDescriptionCard.CloseWindow();
    }

    #endregion

    #region Activate/desactivate buttons

    private void HandleAllButtons()
    {
        HandleMortgage();
        HandleRedeem();
        HandleUpgrade();
        HandleSell();
    }

    private void HandleMortgage()
    {
        leftSideButton.mortgageButton.interactable = _gameManager.CurrentPlayer.HasLand;
    }

    private void HandleUpgrade()
    {
        leftSideButton.upgradeButton.interactable = _gameManager.PlayerOwnsLandSet(_gameManager.CurrentPlayer);
    }

    private void HandleRedeem()
    {
        leftSideButton.redeemButton.interactable = _gameManager.CurrentPlayer.HasMortgagedLand;
    }

    private void HandleSell()
    {
        leftSideButton.sellButton.interactable = _gameManager.PlayerHaveSellableLands(_gameManager.CurrentPlayer);
    }

    #endregion

    #region Change To mode

    public void ChangeToUpgradeLandMode(List<TileLand> tiles)
    {
        SetMidScreen(UPGRADING);
        DeHighlightAllTiles();

        foreach (var tile in tiles)
        {
            tile.SetTileMatColor(MatchSettings.SingletonInstance.HighlightColor);
        }

        ChangeToTopDownView();
    }

    public void ChangeToMortgageMode(List<LandTitleInstance> playerLand)
    {
        SetMidScreen(MORTGAGING);
        DeHighlightAllTiles();
        foreach (var tile in playerLand)
        {
            if (tile.IsMortgaged)
            {
                tile.TileLand.SetTileMatColor(MatchSettings.SingletonInstance.MortgageColor);
            }
            else
            {
                tile.TileLand.SetTileMatColor(MatchSettings.SingletonInstance.HighlightColor);
            }
        }

        ChangeToTopDownView();
    }

    public void ChangeToRedeemMode(List<LandTitleInstance> playerLand)
    {
        SetMidScreen(REDEEMING);
        DeHighlightAllTiles();
        foreach (var tile in playerLand)
        {
            if (tile.IsMortgaged)
            {
                tile.TileLand.SetTileMatColor(MatchSettings.SingletonInstance.MortgageColor);
            }
        }

        ChangeToTopDownView();
    }

    public void ChangeToSellMode(List<TileLand> tiles)
    {
        SetMidScreen(SELLING);

        DeHighlightAllTiles();

        foreach (var land in tiles)
        {
            land.SetTileMatColor(MatchSettings.SingletonInstance.HighlightColor);
        }

        ChangeToTopDownView();
    }

    #endregion

    #region Camera

    public void ChangeCameraView()
    {
        if (_firstPersonView)
        {
            ChangeToTopDownView();
        }
        else
        {
            ChangeToPlayerView(_gameManager.CurrentPlayer);
        }

        _firstPersonView = !_firstPersonView;
    }

    public void ChangeToTopDownView()
    {
        _gameManager.TurnManager.CurrentPlayerTurn.SetCameraActive(false);
        mainCamera.enabled = true;

        mid_BottomScreenRef.ContainerGameObject.anchorMax = new Vector2(0.5f, 0.5f);
        mid_BottomScreenRef.ContainerGameObject.anchorMin = new Vector2(0.5f, 0.5f);
        // mid_BottomScreenRef.ContainerGameObject.
        // mid_BottomScreenRef.ContainerGameObject.position=Vector3.zero;
    }

    public void ChangeToPlayerView(Player player)
    {
        player.SetCameraActive(true);
        mainCamera.enabled = false;
        mid_BottomScreenRef.ContainerGameObject.anchorMax = new Vector2(0.5f, 0f);
        mid_BottomScreenRef.ContainerGameObject.anchorMin = new Vector2(0.5f, 0f);
    }

    #endregion

    public void ExitMode()
    {
        HighlightAllTiles();
        if (_firstPersonView)
            ChangeToPlayerView(_gameManager.TurnManager.CurrentPlayerTurn);
    }

    private void HighLightAllToProperColor()
    {
        foreach (var item in _gameManager.TileManager.TitleOrganized)
        {
            if (item.TitleType == TitleType.Land)
            {
                LandTitleInstance landTitleInstance = (LandTitleInstance)item.TileInstance;
                item.SetTileMatColor(LandStaticInformation.GetColorFromLandType(landTitleInstance.LandDef.LandType));
            }
        }
    }

    private void DeHighlightAllTiles()
    {
        foreach (var item in _gameManager.TileManager.TitleOrganized)
        {
            item.SetTileMatColor(MatchSettings.SingletonInstance.DeHighlightColor);
        }
    }

    private void HighlightAllTiles()
    {
        foreach (var item in _gameManager.TileManager.TitleOrganized)
        {
            if (item.TitleType == TitleType.Land)
            {
                if (item.TileInstance.IsMortgaged)
                {
                    item.SetTileMatColor(MatchSettings.SingletonInstance.MortgageColor);
                    continue;
                }

                if (MatchSettings.SingletonInstance.UseAppropriateColors)
                {
                    LandTitleInstance landTitleInstance = (LandTitleInstance)item.TileInstance;
                    item.SetTileMatColor(
                        LandStaticInformation.GetColorFromLandType(landTitleInstance.LandDef.LandType));
                    continue;
                }
            }


            item.SetTileMatColor(MatchSettings.SingletonInstance.HighlightColor);
        }
    }

    private void CloseMidScreen()
    {
        midScreenRef.MidGameObject.SetActive(false);
    }

    private void SetMidScreen(string text)
    {
        midScreenRef.MidGameObject.SetActive(true);
        midScreenRef.MidText.text = text;
    }

    #region Button Functions

    private bool _tradeMenuOpen = false;

    public void TradeMenuClicked()
    {
        if (_tradeMenuOpen)
        {
            //hide it
            ChangeTradeCardsActivity(false);
        }
        else
        {
            ChangeTradeCardsActivity(true);
            CreateMiniPlayerCard();
        }

        _tradeMenuOpen = !_tradeMenuOpen;
    }

    private void CreateMiniPlayerCard()
    {
        if (_activePlayerInfos.Count < _gameManager.TurnManager.ActivePlayers.Count() - 1)
        {
            Player currentPlayer = _gameManager.TurnManager.CurrentPlayerTurn;
            foreach (var player in _gameManager.TurnManager.ActivePlayers)
            {
                if (player != currentPlayer)
                {
                    SmallPlayerInfoCard card = Instantiate<SmallPlayerInfoCard>(smallPlayerInfoCard_Prefab, default,
                        default, tradePlayerParent);
                    SmallPlayerInfoCard.SmallPlayerInfoCardHandle smallPlayerInfoCardHandle =
                        new SmallPlayerInfoCard.SmallPlayerInfoCardHandle()
                        {
                            playerName = player.PlayerName,
                            playerColor = player.PlayerColor
                        };
                    card.Init(
                        new InitalizationHandle<SmallPlayerInfoCard.SmallPlayerInfoCardHandle>(
                            smallPlayerInfoCardHandle));
                    card.ClickableButton.onClick.AddListener((() => PlayerClicked(player)));

                    _activePlayerInfos.Add(player, card);
                }
            }
        } //don't create more use existing ones
        //just simply disable who lost
        else
        {
            foreach (var playerInfo in _activePlayerInfos)
            {
                if (playerInfo.Key.HasLost)
                {
                    playerInfo.Value.ClickableButton.interactable = false;
                }
            }
        }
    }

    private void ChangeTradeCardsActivity(bool isActive)
    {
        tradePlayerParent.gameObject.SetActive(isActive);
        // foreach (var playerInfo in _activePlayerInfos)
        // {
        //    playerInfo.Value.gameObject.SetActive(false);
        // }
    }

    private void PlayerClicked(Player player)
    {
        _gameManager.OpenTradeMenu(player);
        ChangeTradeCardsActivity(false);
        _tradeMenuOpen = false;
    }

    public void SwitchAIActive()
    {
        _gameManager.SwitchAIState();
        if (_gameManager.CurrentPlayer.AIController.Active)
        {
            switchAutoPlayButtonText.text = DEACTIVATE_AI;
        }
        else
        {
            switchAutoPlayButtonText.text = ACTIVATE_AI;
        }
    }

    #endregion

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Pressed Object eventData.pointerPress {eventData.pointerPress.name}");
        if (eventData.pointerPress)
        {
            Debug.Log($"Pressed Object eventData.pointerPress {eventData.pointerPress.name}");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }
}