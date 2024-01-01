using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LandInformationMenu : MenuBase
{
    [SerializeField] private TextMeshProUGUI landName;
    [SerializeField] private Transform body;


    private List<SmallPlayerInfoCard> _infoCards = new List<SmallPlayerInfoCard>();


    public override void Init<T>(InitalizationHandle<T> handle)
    {
        LandTitleInstance landTitleInstance = (LandTitleInstance)(handle.Data as object);

        landName.text = landTitleInstance.LandDef.LandName;
        if (_infoCards.Count < landTitleInstance.LandDef.Rent.Length)
        {
            foreach (var VARIABLE in _infoCards)
            {
                Destroy(VARIABLE.gameObject);
            }
            _infoCards.Clear();
            int i = 0;
            CreateLandInfo(Color.black, landTitleInstance.LandDef.Rent[i]);
            i++;
            for (; i < landTitleInstance.LandDef.Rent.Length; i++)
            {
                CreateLandInfo(MatchSettings.SingletonInstance.LandColors[i - 1], landTitleInstance.LandDef.Rent[i]);
            }
        }
        else
        {
            int i = 0;
            SmallPlayerInfoCard.SmallPlayerInfoCardHandle smallPlayerInfoCardHandle =
                new SmallPlayerInfoCard.SmallPlayerInfoCardHandle();
            smallPlayerInfoCardHandle.playerName = landTitleInstance.LandDef.Rent[i].ToString();
            smallPlayerInfoCardHandle.playerColor = Color.black;
            _infoCards[i]
                .Init(new InitalizationHandle<SmallPlayerInfoCard.SmallPlayerInfoCardHandle>(
                    smallPlayerInfoCardHandle));
            i++;
            for (; i < landTitleInstance.LandDef.Rent.Length; i++)
            {
                smallPlayerInfoCardHandle.playerName = landTitleInstance.LandDef.Rent[i].ToString();
                smallPlayerInfoCardHandle.playerColor = MatchSettings.SingletonInstance.LandColors[i - 1];
                _infoCards[i]
                    .Init(new InitalizationHandle<SmallPlayerInfoCard.SmallPlayerInfoCardHandle>(
                        smallPlayerInfoCardHandle));
            }

            if (_infoCards.Count > landTitleInstance.LandDef.Rent.Length )
            {
                var itemToRemove = _infoCards.Last();
                Destroy(itemToRemove.gameObject);
                _infoCards.Remove(itemToRemove);
            }
        }

        gameObject.SetActive(true);
    }

    private void CreateLandInfo(Color color, int Rent)
    {
        SmallPlayerInfoCard.SmallPlayerInfoCardHandle smallPlayerInfoCardHandle =
            new SmallPlayerInfoCard.SmallPlayerInfoCardHandle();
        var LandInfo = Instantiate(MatchSettings.SingletonInstance.PlayerInfoCard, default, default, body);
        smallPlayerInfoCardHandle.playerName = Rent.ToString();
        smallPlayerInfoCardHandle.playerColor = color;
        LandInfo.Init(
            new InitalizationHandle<SmallPlayerInfoCard.SmallPlayerInfoCardHandle>(smallPlayerInfoCardHandle));
        _infoCards.Add(LandInfo);
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