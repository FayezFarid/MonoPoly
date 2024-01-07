using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public class PlayerInfo : CardBase
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Image playerColor;
    [SerializeField] private Image prisonImage;
    private Button _button;
    private Image _parentImage;

    void Awake()
    {
        _parentImage = GetComponent<Image>();
        _button = GetComponent<Button>();
    }
    public override void Init<T>(InitalizationHandle<T> initalizationHandle)
    {
        Player playerData = initalizationHandle.Data as Player;
        SetMoney(playerData.Money);
        SetPlayerName(playerData.PlayerName);
        playerColor.color = playerData.PlayerColor;
    }

    public void SetMoney(int money)
    {
        moneyText.text = money.ToString();
    }

    public void SetPlayerName(string playerName)
    {
        playerNameText.text = playerName;
    }

    public void SetColor(Color color)
    {
        _parentImage.color = color;
    }

    public void SetPrisonState(bool state)
    {
        prisonImage.gameObject.SetActive(state);
    }

    public void AssignOnClick(Action<PlayerInfo> onclickEvent)
    {
        _button.onClick.AddListener( () => onclickEvent?.Invoke(this) );
    }


   
}