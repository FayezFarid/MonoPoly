using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MMVariablesMenu : MonoBehaviour
{
    [Serializable]
    public struct MoneyRef
    {
        public TMP_InputField initialMoney;
        public TMP_InputField startMoney;
        public TMP_InputField prisonMoney;
    }

    [Serializable]
    public struct BidRef
    {
        public TMP_InputField bidIncreaseMoney;
    }

    [SerializeField] private MoneyRef moneyRef;
    [SerializeField] private BidRef bidRef;

    private void OnEnable()
    {
        MatchSettings matchSettings = MatchSettings.SingletonInstance;

        moneyRef.initialMoney.text = matchSettings.InitalMoney.ToString();
        moneyRef.startMoney.text = matchSettings.WholeLoopMoney.ToString();
        moneyRef.prisonMoney.text = matchSettings.PrisonFee.ToString();
        int bidMoney =(int)Mathf.Ceil( matchSettings.BidIncreasePercentage * 100);
        bidRef.bidIncreaseMoney.text =bidMoney.ToString()+ " %";
    }

    public void Confirm()
    {
        MatchSettings matchSettings = MatchSettings.SingletonInstance;

        matchSettings.InitalMoney = int.Parse(moneyRef.initialMoney.text);
        matchSettings.WholeLoopMoney = int.Parse(moneyRef.startMoney.text);
        matchSettings.PrisonFee = int.Parse(moneyRef.prisonMoney.text);

        Close();
        // matchSettings.BidIncreasePercentage=
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}