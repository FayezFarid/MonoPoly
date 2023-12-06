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
    public static Dictionary<LandTypes, int> LandType_Upgrade = new FlexibleDictionary<LandTypes, int>()
    {
        { LandTypes.Blue, 50 },
        { LandTypes.Brown, 50 },
        { LandTypes.BlueSky,100  },
        { LandTypes.Green,100 },
        { LandTypes.Yellow,150 },
        { LandTypes.Purple,150 },
        { LandTypes.Orange,200 },
        { LandTypes.Red,200},
    };
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
    [Header("Don't fill for station")]
    [SerializeField] protected int[] rent;
 

    public string LandName => landName;

    public int LandValue => landValue;

    public virtual int MoneyToUpgrade => LandStaticInformation.LandType_Upgrade[LandType];

    public int MortgageRevenue => mortgageRevenue;

    public int RedeemFee => redeemFee;

    public virtual int[] Rent => rent;

    public LandTypes LandType => landType;

}

public class LandTitleInstance : TileInstance
{
 
    private LandTileDefinition _landDef;
    public LandTileDefinition LandDef => _landDef;
    public int CurrentRank;
    public bool IsMortgaged;
    private Player owner;

    public Player Owner
    {
        get => owner;
        set
        {
            owner = value;
            OnLandOwnerChanged?.Invoke(value);
        }
    }
    
    public bool IsOwned => Owner != null;

    public bool IsStation => LandDef.LandType == LandTypes.Station;
    public int GetRent => _landDef.Rent[CurrentRank];


  
    public LandTitleInstance(TileDefinition landdef)
    {
        
        _landDef =(LandTileDefinition) landdef;
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