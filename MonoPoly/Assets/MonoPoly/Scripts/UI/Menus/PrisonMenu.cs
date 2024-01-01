using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrisonMenu : MenuBase
{
    [SerializeField] private Button useCardButton;
    [SerializeField] private Button payFee;
    [SerializeField] private  GameManager _gameManager;
    
    public struct InitPrisonHandle
    {
        public bool CanPayForPrison;
        public bool CanUseCard;
      
    }

    public void Start()
    {
        payFee.GetComponentInChildren<TextMeshProUGUI>().text = "Pay " + MatchSettings.SingletonInstance.PrisonFee;
    }

    public override void Init<T>(InitalizationHandle<T> handle)
    {
        InitPrisonHandle initPrisonHandle = (InitPrisonHandle)(handle.Data as object);
        payFee.interactable = initPrisonHandle.CanPayForPrison;
        useCardButton.interactable = initPrisonHandle.CanUseCard;
        gameObject.SetActive(true);

    }

    public void UseCard()
    {
        _gameManager.PlayerUsesCard();
    }

    public void PayFee()
    {
        _gameManager.CurrentPaysPrisonFee();
    }

    public void Roll()
    {
        _gameManager.PlayerRollForPrison();
        
    }
    public override void CloseWindow()
    {
       gameObject.SetActive(false);
    }
}