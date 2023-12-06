using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

   //TODO: player buff and debuffs
   
   //Player should be coming from here or sorted out in the subcriber
   public Action<int /*Previous*/,int /*next*/,int /*modifier*/> MoneyChanged;
   public Action PlayerBroke;
   
   private string _playerName;
   private Color32 playerColor;

   public string PlayerName
   {
      get => _playerName;
      set => _playerName = value;
   }
   private int _currentPosition;

   private int _money;
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
         if(_material!=null)
            _material.color = PlayerColor;
         
      }
   }

   private List<LandTitleInstance> _ownedLands=new List<LandTitleInstance>();
   public List<LandTitleInstance> OwnedLands => _ownedLands;
   
   
   [SerializeField] private Camera PlayerCamera;
   private Material _material;


   

   public void Start()
   {
      _material = GetComponentInChildren<Renderer>().material;
      _material.color = PlayerColor;
   }

   public bool PlayerOwnsLand(LandTitleInstance landTitleInstance)
   {
      return _ownedLands.Contains(landTitleInstance);
   }

   public void ReduceMoney(int amount)
   {
      _money -= amount;
      MoneyChanged?.Invoke(_money+amount,_money,-amount);
      if (_money < 0)
      {
         PlayerBroke?.Invoke();
      }
   }
   
   public void IncreaseMoney(int amount)
   {
      _money += amount;
      MoneyChanged?.Invoke(_money-amount,_money,amount);
   }

   public void BuyLand(LandTitleInstance landTitleInstance)
   {
      Debug.Log($"Player bought land {landTitleInstance} ");
      ReduceMoney(landTitleInstance.LandDef.LandValue);
      landTitleInstance.Owner = this;
      _ownedLands.Add(landTitleInstance);
   }
   public void PayPlayer(Player player,int amount)
   {
      ReduceMoney(amount);
      player.IncreaseMoney(amount);
   }
   public void PlayerTurnStarted()
   {
      PlayerCamera.enabled = true;
   }
   public void PlayerTurnEnded()
   {
      PlayerCamera.enabled = false;
   }

   public override string ToString()
   {
      return $"Player [{_playerName}] Position [{_currentPosition} Money [{_money}]";
   }
}

