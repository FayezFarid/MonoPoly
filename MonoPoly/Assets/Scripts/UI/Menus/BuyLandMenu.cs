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
    private UnityAction<Player,LandTitleInstance> _buyLandAction;
    private UnityAction _auctionAction;
    private LandTitleInstance _titleInstance;
    private Player _player;

    public void EnableMenu(UnityAction<Player,LandTitleInstance> buyLandAction, UnityAction auctionAction, LandTitleInstance titleInstance,Player player)
    {
        gameObject.SetActive(true);
        _player = player;
        _buyLandAction = buyLandAction;
        _auctionAction = auctionAction;
        _titleInstance = titleInstance;
        buyLandButton.onClick.AddListener(() =>
        {
            _buyLandAction?.Invoke(_player,_titleInstance);
            CleanUp();
        });
        auctionLandButton.onClick.AddListener( () =>
        {
            _auctionAction?.Invoke();
            CleanUp();
        });
    }

    private void CleanUp()
    {
        _player = null;
        _buyLandAction = null;
        _auctionAction = null;
        buyLandButton.onClick.RemoveAllListeners();
        auctionLandButton.onClick.RemoveAllListeners();
        _titleInstance = null;
        gameObject.SetActive(false);
        
    }
}
