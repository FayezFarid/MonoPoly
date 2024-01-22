using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum LandTypes
{
    Blue,
    Brown,
    BlueSky,
    Green,
    Yellow,
    Purple,
    Orange,
    Red,
    Station,
}

public static class LandStaticInformation
{
    public static Dictionary<LandTypes, int> LandType_Upgrade = new Dictionary<LandTypes, int>()
    {
        { LandTypes.Blue, 25 },
        { LandTypes.Brown, 30 },
        { LandTypes.BlueSky, 100 },
        { LandTypes.Green, 120 },
        { LandTypes.Yellow, 150 },
        { LandTypes.Purple, 150 },
        { LandTypes.Orange, 200 },
        { LandTypes.Red, 220 },
    };

    public static Color ColorBrown = new Color(85, 47, 0);
    public static Color ColorPurple = new Color(128, 0, 128);
    public static Color ColorOrange = new Color(255, 140, 0);

    public static Color GetColorFromLandType(LandTypes landType)
    {
        if (landType == LandTypes.Blue)
        {
            return Color.blue;
        }

        if (landType == LandTypes.Brown)
        {
            return ColorBrown;
        }

        if (landType == LandTypes.BlueSky)
        {
            return Color.cyan;
        }

        if (landType == LandTypes.Green)
        {
            return Color.green;
        }

        if (landType == LandTypes.Yellow)
        {
            return Color.yellow;
        }

        if (landType == LandTypes.Purple)
        {
            return ColorPurple;
        }

        if (landType == LandTypes.Orange)
        {
            return ColorOrange;
        }

        if (landType == LandTypes.Red)
        {
            return Color.red;
        }

        if (landType == LandTypes.Station)
        {
            return Color.white;
        }

        return Color.black;
    }
}

[CreateAssetMenu(menuName = "Monopoly/Land Title Definition")]
public class LandTileDefinition : TileDefinition
{
    //TODO: Add texture mat smth for visual
    [SerializeField] protected string landName;

    [SerializeField] protected int landValue;

    // [SerializeField] protected int moneyToUpgrade;
    [SerializeField] protected int mortgageRevenue;
    [SerializeField] protected int redeemFee;
    [SerializeField] protected LandTypes landType;
    [SerializeField] private bool IsFinalTile;

    [Header("Don't fill for station")] [SerializeField]
    protected int[] rent;
    

    // private void OnEnable()
    // {
    //     rent = new int[5];
    //     rent[0] = landValue / 5;
    //     rent[1] = landValue / 2;
    //     rent[2] = Mathf.FloorToInt(landValue * 1.5f) ;
    //     rent[3] = Mathf.FloorToInt(landValue * 3) ;
    //     rent[4] = Mathf.FloorToInt(landValue * 5) ;
    // }

    public string LandName => landName;

    public int LandValue => landValue;

    public virtual bool CanSellHouses => true;
    public virtual int MoneyToUpgrade => LandStaticInformation.LandType_Upgrade[LandType];
    public virtual int MoneyFromSellHouse => LandStaticInformation.LandType_Upgrade[LandType] / 2;

    public int MortgageRevenue => mortgageRevenue;

    public int RedeemFee => redeemFee;

    public virtual int[] Rent => rent;

    public LandTypes LandType => landType;
}

[Serializable]
public class LandTitleInstance : TileInstance
{
    public static Action<LandTitleInstance> OnLandUpgraded;
    public static Action<LandTitleInstance> OnLandMortgaged;
    public static Action<LandTitleInstance> OnLandRedeem;

    [SerializeField] private LandTileDefinition _landDef;
    public LandTileDefinition LandDef => _landDef;
    private Player owner;

    private Index _currentRank;

    private bool _mortgaged;
    public override bool IsMortgaged => _mortgaged;


    public Player Owner
    {
        get => owner;
        set
        {
            owner = value;
            OnLandOwnerChanged?.Invoke(this, value);
        }
    }

    public bool IsOwned => Owner != null;

    public int CurrentRank => _currentRank.CurrentIndex;
    public virtual int GetRent => _landDef.Rent[_currentRank.CurrentIndex];
    public bool HasHouses => _currentRank.CurrentIndex > 0;

    public bool IsStation => LandDef.LandType == LandTypes.Station;

    public int GetMaxRentRank => _landDef.Rent.Length;
    public bool AtMaxRank => _currentRank.CurrentIndex == _landDef.Rent.Length - 1;

    public LandTitleInstance(TileDefinition landdef, TileLand tileLand)
    {
        _tileLand = tileLand;
        _landDef = (LandTileDefinition)landdef;
        _currentRank = new Index(0, _landDef.Rent.Length, false);
        _mortgaged = false;
    }

    public override void UpgradeLand()
    {
        //todo: iNDEX STRUCT
        if (AtMaxRank)
        {
            SpicyHarissaLogger.Log($" Land [{_landDef.LandName}] at Max ", LogLevel.Standard);
            return;
        }

        _currentRank.MoveNextIndex();
        OnLandUpgraded?.Invoke(this);
    }

    public bool DownGradeLand()
    {
        if (_currentRank.CurrentIndex == 0)
        {
            SpicyHarissaLogger.LogError($" Land [{_landDef.LandName}] at lowest level but trying to downgrade ");
            return false;
        }

        if (!_landDef.CanSellHouses)
        {
            return false;
        }

        _currentRank.GoPrevious();
        OnLandUpgraded?.Invoke(this);
        return true;
    }

    public void MortgageLand()
    {
        _mortgaged = true;
        OnLandMortgaged?.Invoke(this);
    }

    public void RedeemLand()
    {
        _mortgaged = false;
        OnLandRedeem?.Invoke(this);
    }

    public override void OnLandedUpon()
    {
        throw new System.NotImplementedException();
    }

    public override string ToString()
    {
        return $"Land Name= {_landDef.LandName} LandValue= {_landDef.LandValue}";
    }
}