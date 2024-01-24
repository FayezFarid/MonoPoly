using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LandInformationMenu : MenuBase
{
    public static string LAND_VALUE = "Land Value: ";
    public static string LAND_NULL_UPGRADES = "Empty Land Rent: ";
    [SerializeField] private TextMeshProUGUI landName;
    [SerializeField] private Transform body;
    [SerializeField] private TextMeshProUGUI mortgageText;
    [SerializeField] private TextMeshProUGUI redeemText;


    private List<SmallPlayerInfoCard> _infoCards = new List<SmallPlayerInfoCard>();


    public override void Init<T>(InitalizationHandle<T> handle)
    {
        LandTitleInstance landTitleInstance = (LandTitleInstance)(handle.Data as object);

        landName.text = landTitleInstance.LandDef.LandName;
        //requires brain cells idc just recreate
        //Create new
        // if (_infoCards.Count < landTitleInstance.LandDef.Rent.Length)
        // {
            foreach (var VARIABLE in _infoCards)
            {
                Destroy(VARIABLE.gameObject);
            }
            _infoCards.Clear();

            redeemText.text = landTitleInstance.LandDef.RedeemFee.ToString();
            mortgageText.text = landTitleInstance.LandDef.MortgageRevenue.ToString();
            int i = 0;
            //Land Value 
            CreateLandInfo_TextOnly(LAND_VALUE + landTitleInstance.LandDef.LandValue);
            CreateLandInfo_TextOnly(LAND_NULL_UPGRADES + landTitleInstance.LandDef.Rent[i]);
            i++;
            for (; i < landTitleInstance.LandDef.Rent.Length; i++)
            {
                CreateLandInfo(MatchSettings.SingletonInstance.LandColors[i - 1], landTitleInstance.LandDef.Rent[i]);
            }
        // } //Use already existing 
        // else
        // {
        //     int i = 0;
        //     SmallPlayerInfoCard.SmallPlayerInfoCardHandle smallPlayerInfoCardHandle =
        //         new SmallPlayerInfoCard.SmallPlayerInfoCardHandle();
        //     // smallPlayerInfoCardHandle.playerName = landTitleInstance.LandDef.Rent[i].ToString();
        //     // smallPlayerInfoCardHandle.playerColor = Color.black;
        //     // _infoCards[i]
        //     //     .Init(new InitalizationHandle<SmallPlayerInfoCard.SmallPlayerInfoCardHandle>(
        //     //         smallPlayerInfoCardHandle));
        //     Initalize_TextOnly(_infoCards[i],LAND_NULL_UPGRADES + landTitleInstance.LandDef.Rent[i]);
        //     i++;
        //     for (; i < landTitleInstance.LandDef.Rent.Length; i++)
        //     {
        //         smallPlayerInfoCardHandle.playerName = landTitleInstance.LandDef.Rent[i].ToString();
        //         smallPlayerInfoCardHandle.playerColor = MatchSettings.SingletonInstance.LandColors[i - 1];
        //         _infoCards[i]
        //             .Init(new InitalizationHandle<SmallPlayerInfoCard.SmallPlayerInfoCardHandle>(
        //                 smallPlayerInfoCardHandle));
        //     }
        //
        //     if (_infoCards.Count > landTitleInstance.LandDef.Rent.Length)
        //     {
        //         var itemToRemove = _infoCards.Last();
        //         Destroy(itemToRemove.gameObject);
        //         _infoCards.Remove(itemToRemove);
        //     }
        // }

        gameObject.SetActive(true);
    }

    private void CreateLandInfo_TextOnly(string message)
    {
        var landInfo = Instantiate(MatchSettings.SingletonInstance.OneTextCard, default, default, body);
        Initalize_TextOnly(landInfo, message);
        _infoCards.Add(landInfo);
    }

    private void Initalize_TextOnly(SmallPlayerInfoCard landInfo, string message)
    {
        SmallPlayerInfoCard.SmallPlayerInfoCardHandle smallPlayerInfoCardHandle =
            new SmallPlayerInfoCard.SmallPlayerInfoCardHandle();
        smallPlayerInfoCardHandle.playerName = message;
        landInfo.Init(
            new InitalizationHandle<SmallPlayerInfoCard.SmallPlayerInfoCardHandle>(smallPlayerInfoCardHandle));
    }

    private void CreateLandInfo(Color color, int rent)
    {
 
        var landInfo = Instantiate(MatchSettings.SingletonInstance.PlayerInfoCard, default, default, body);
        InitializeLandInfo(landInfo, color, rent);
        _infoCards.Add(landInfo);
    }

    private void InitializeLandInfo(SmallPlayerInfoCard landInfo, Color color, int rent)
    {
        SmallPlayerInfoCard.SmallPlayerInfoCardHandle smallPlayerInfoCardHandle =
            new SmallPlayerInfoCard.SmallPlayerInfoCardHandle();
        smallPlayerInfoCardHandle.playerName = rent.ToString();
        smallPlayerInfoCardHandle.playerColor = color;
        landInfo.Init(
            new InitalizationHandle<SmallPlayerInfoCard.SmallPlayerInfoCardHandle>(smallPlayerInfoCardHandle));
    }

    public override void CloseWindow()
    {
        //dont destroy re-use
        // foreach (var VARIABLE in _infoCards)
        // {
        //     Destroy(VARIABLE.gameObject);
        // }
        // _infoCards.Clear();
        gameObject.SetActive(false);
    }
}