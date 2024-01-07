using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IMoneyTrader
{
    bool CanNotAfford(int amount);

    void ReduceMoney(int amount);

    //Returns new changed Amount only when it's needed can be used from return
    int IncreaseMoney(int amount);
    void PayPlayer(IMoneyTrader player, int amount);
}

public class Player : MonoBehaviour, IMoneyTrader
{
    public static Action<Player> OnPlayerBroke;
    public static Action<Player> OnPlayerEscapedBroke;
    public static Action<Player> OnPlayerLost;

    //Player should be coming from here or sorted out in the subcriber
    public Action<int /*Previous*/, int /*next*/, int /*modifier*/> MoneyChanged;


    //Whack need something better


    #region Standard Attributes and land

    [SerializeField] private AIController aiController;

    private string _playerName;
    private Color32 playerColor;
    private int _currentPosition;

    private bool _canRollAgain = false;
    private int _money;
    private bool _lost;

    public AIController AIController => aiController;

    public string PlayerName
    {
        get => _playerName;
        set => _playerName = value;
    }


    public int Money
    {
        get => _money;
    }

    public int CurrentPosition
    {
        get => _currentPosition;
        set => _currentPosition = value;
    }

    public Color32 PlayerColor
    {
        get => playerColor;
        set
        {
            playerColor = value;
            if (_material != null)
                _material.color = PlayerColor;
        }
    }


    public bool HasLost
    {
        get
        {
            //I Will give the benefit of 0
            return _lost;
        }
    }


    public bool HasMortgagedLand
    {
        get
        {
            foreach (var land in _ownedLands)
            {
                if (land.IsMortgaged)
                    return true;
            }

            return false;
        }
    }

    public bool OutOfMoney => _money <= 0;
    public bool HasLand => _ownedLands.Count > 0;

    public int OwnedStations
    {
        get
        {
            if (!_ownedLandWithSameType.ContainsKey(LandTypes.Station))
                return 0;
            return _ownedLandWithSameType[LandTypes.Station].Count;
        }
    }

    [SerializeField] private List<LandTitleInstance> _ownedLands = new List<LandTitleInstance>();

    private Dictionary<LandTypes, List<LandTitleInstance>> _ownedLandWithSameType =
        new Dictionary<LandTypes, List<LandTitleInstance>>();


    public List<LandTitleInstance> OwnedLands => _ownedLands;

    public Dictionary<LandTypes, List<LandTitleInstance>> OwnedLandWithSameType => _ownedLandWithSameType;

    #endregion


    [SerializeField] private Camera PlayerCamera;
    private Material _material;
    public List<StatusEffectInstance> StatusEffects = new List<StatusEffectInstance>();

    //TODO:Change this to status effect
    public bool IsFirstRoll { get; set; } = true;

    public bool CanRollAgain
    {
        get => _canRollAgain;
        set => _canRollAgain = value;
    }


    public void Start()
    {
        _material = GetComponentInChildren<Renderer>().material;
        _material.color = PlayerColor;
    }

    private void OnEnable()
    {
        StatusEffectInstance.OnEffectEnded += OnEffectEnded;
    }

    private void OnDisable()
    {
        StatusEffectInstance.OnEffectEnded -= OnEffectEnded;
    }

    #region Money

    public bool CanNotAfford(int amount) => amount >= _money;

    public void ReduceMoney(int amount)
    {
        foreach (var effect in StatusEffects)
        {
            effect.OnMoneyRemoved(ref amount);
        }

        _money -= amount;
        MoneyChanged?.Invoke(_money + amount, _money, -amount);
        if (_money <= 0)
        {
            SpicyHarissaLogger.Log($"{this} Player is broke ", LogLevel.Standard);
            OnPlayerBroke?.Invoke(this);
        }
    }

    public int IncreaseMoney(int amount)
    {
        foreach (var effect in StatusEffects)
        {
            effect.OnMoneyAdd(ref amount);
        }

        _money += amount;
        if (_money > 0 && _money - amount <= 0)
        {
            OnPlayerEscapedBroke?.Invoke(this);
        }

        MoneyChanged?.Invoke(_money - amount, _money, amount);
        return amount;
    }

    public void PayPlayer(IMoneyTrader player, int amount)
    {
        //Don't reduce first maybe player can not gain money
        int NewAmount = player.IncreaseMoney(amount);
        ReduceMoney(NewAmount);
    }

    #endregion

    #region Land

    public bool PlayerOwnsLand(LandTitleInstance landTitleInstance)
    {
        return _ownedLands.Contains(landTitleInstance);
    }


    public void BuyLand(LandTitleInstance landTitleInstance)
    {
        if (landTitleInstance.Owner != null)
        {
            SpicyHarissaLogger.Log($"{landTitleInstance.Owner} already owns land {landTitleInstance} ",
                LogLevel.Standard);
            return;
        }

        if (_ownedLands.Contains(landTitleInstance))
        {
            SpicyHarissaLogger.Log($"{this} already owns land {landTitleInstance} ", LogLevel.Standard);
            return;
        }

        SpicyHarissaLogger.Log($"Player bought land {landTitleInstance} ", LogLevel.Standard);

        ReduceMoney(landTitleInstance.LandDef.LandValue);
        GainLand(landTitleInstance);
    }

    public void PlayerLosesLand(LandTitleInstance landTitleInstance)
    {
        _ownedLands.Remove(landTitleInstance);
        _ownedLandWithSameType.RemoveLandTileInstance(landTitleInstance);
        landTitleInstance.Owner = null;
    }

    public void GainLand(LandTitleInstance landTitleInstance)
    {
        _ownedLands.Add(landTitleInstance);
        OwnedLandWithSameType.AddLandTileInstance(landTitleInstance);
        landTitleInstance.Owner = this;
    }

    #endregion


    #region Upgrade/Mortgage/Redeem

    public bool UpgradePlayerLand(TileInstance tileInstance)
    {
        LandTitleInstance landTitleInstance = (LandTitleInstance)tileInstance;
        if (CanNotAfford(landTitleInstance.LandDef.MoneyToUpgrade))
        {
            return false;
        }

        if (LandStaticInformation.LandType_Upgrade.TryGetValue(landTitleInstance.LandDef.LandType, out var value))
        {
            ReduceMoney(value);
            tileInstance.UpgradeLand();
            return true;
        }

        return false;
    }

    public void SellHouse(TileInstance tileInstance)
    {
        LandTitleInstance landTitleInstance = (LandTitleInstance)tileInstance;
        bool result = landTitleInstance.DownGradeLand();
        if (result)
        {
            IncreaseMoney(landTitleInstance.LandDef.MoneyFromSellHouse);
        }
    }

    public void MortgagePlayerLand(LandTitleInstance titleInstance)
    {
        if (titleInstance.IsMortgaged)
        {
            SpicyHarissaLogger.Log($"{this} already Mortaged land {titleInstance} ", LogLevel.Standard);
            return;
        }

        IncreaseMoney(titleInstance.LandDef.MortgageRevenue);
        titleInstance.MortgageLand();
    }

    public bool RedeemPlayerLand(LandTitleInstance titleInstance)
    {
        if (!titleInstance.IsMortgaged)
        {
            SpicyHarissaLogger.Log($"{this} already Redeemed land {titleInstance} ", LogLevel.Standard);
            return false;
        }

        if (CanNotAfford(titleInstance.LandDef.RedeemFee))
        {
            return false;
        }

        ReduceMoney(titleInstance.LandDef.RedeemFee);
        titleInstance.RedeemLand();
        return true;
    }

    #endregion

    #region Player Turn

    public void PlayerTurnStarted()
    {
        foreach (var effect in StatusEffects)
        {
            effect.OnPlayerTurnStarted();
        }
    }

    public void PlayerTurnEnded()
    {
        //make copy because when iterating, some effect may end.
        List<StatusEffectInstance> copyStatusEffect = new List<StatusEffectInstance>(StatusEffects);
        foreach (var effect in copyStatusEffect)
        {
            effect.OnPlayerTurnEnded();
        }
    }

    #endregion

    #region StatusEffect

    public void ApplyEffectToSelf(StatusEffectDefinition definition)
    {
        StatusEffectInstance instance = new StatusEffectInstance(definition);
        StatusEffects.Add(instance);
    }

    public void RemoveEffectByType(EffectType type, bool removeAll = false)
    {
        // StatusEffectInstance instance = null;
        foreach (var effect in StatusEffects)
        {
            if (effect.EffectType == type)
            {
                //If Removing it also Consider it ended we don't reuse.
                effect.EffectEnded();
                StatusEffects.Remove(effect);
                break;
            }
        }
    }

    public bool HaveEffectOfType(EffectType type)
    {
        foreach (var effect in StatusEffects)
        {
            if (effect.EffectType == type)
            {
                return true;
            }
        }

        return false;
    }

    private void OnEffectEnded(StatusEffectInstance effect)
    {
        StatusEffects.Remove(effect);
    }

    #endregion

    public void PlayerForfeit()
    {
        _lost = true;
        List<LandTitleInstance> lands = new List<LandTitleInstance>(_ownedLands);
        foreach (var land in lands)
        {
            PlayerLosesLand(land);
        }

        StatusEffects.Clear();
        OnPlayerLost?.Invoke(this);

        //too much in a  single frame
        // GC.Collect();
    }

    public void SetCameraActive(bool ActiveState)
    {
        PlayerCamera.enabled = ActiveState;
    }

    public override string ToString()
    {
        return $"Player [{_playerName}] Position [{_currentPosition} Money [{_money}]";
    }
}