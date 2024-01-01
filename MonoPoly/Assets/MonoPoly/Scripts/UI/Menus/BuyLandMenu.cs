using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//TODO: Base class Menu that goes to the side etc basic white chick stuff
public class BuyLandMenu : MenuBase
{
    [SerializeField] private Button buyLandButton;
    [SerializeField] private Button auctionLandButton;
    [SerializeField] private LandInformationMenu landInformationMenu;
    
    private UnityAction<Player, LandTitleInstance> _buyLandAction;
    private UnityAction _auctionAction;
    private LandTitleInstance _titleInstance;
    private Player _player;

    //todo: make this in Base and add generic handle ?
    public void EnableMenu(UnityAction<Player, LandTitleInstance> buyLandAction, UnityAction auctionAction,
        LandTitleInstance titleInstance, Player player)
    {
        gameObject.SetActive(true);
        _player = player;
        _auctionAction = auctionAction;
        _titleInstance = titleInstance;
        
        landInformationMenu.Init(new InitalizationHandle<LandTitleInstance>(titleInstance));
        
        auctionLandButton.onClick.AddListener(() =>
        {
            _auctionAction?.Invoke();
            ResetWindow();
        });
        
        if (buyLandAction != null)
        {

            buyLandButton.interactable = true;
            _buyLandAction = buyLandAction;
            buyLandButton.onClick.AddListener(() =>
            {
                _buyLandAction?.Invoke(_player, _titleInstance);
                ResetWindow();
            });
        }
        else
        {
            buyLandButton.interactable = false;
        }


       
    }

    public override void Init<T>(InitalizationHandle<T> handle)
    {
        throw new NotImplementedException();
    }

    public override void CloseWindow()
    {
        ResetWindow();
    }

    protected override void ResetWindow()
    {
        landInformationMenu.CloseWindow();
        _player = null;
        _buyLandAction = null;
        _auctionAction = null;
        buyLandButton.onClick.RemoveAllListeners();
        auctionLandButton.onClick.RemoveAllListeners();
        _titleInstance = null;
        gameObject.SetActive(false);
    }
}