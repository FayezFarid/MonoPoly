using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static int GoPrisonTile = 29;
    public static int PrisonTile = 9;
    public static int ParkingTilePosition = 19;
    
    
    public List<Tile> TitleOrganized;
    
    public Dictionary<LandTypes, List<LandTitleInstance>> LandInstancePairedLandType=new Dictionary<LandTypes, List<LandTitleInstance>>();

    public Dictionary<LandTypes, int> LandTypePairedCount=new Dictionary<LandTypes, int>();

    public Tile this[int index]
    {
        get { return TitleOrganized[index]; }
    }
    public Tile ParkingTile => TitleOrganized[ParkingTilePosition];

    public int TileCount => TitleOrganized.Count;
    
    private GameManager _gameManager;
    
    public void InitalizeWithGameManager(GameManager gameManager)
    {
        _gameManager = GetComponent<GameManager>();
        // SortTitles();
    }

    public void SortTitles()
    {
        LandTitleInstance.OnLandUpgraded += OnLandUpgraded;
        LandTitleInstance.OnLandMortgaged += LandMortgaged;
        LandTitleInstance.OnLandRedeem += OnLandRedeem;
        
        
        Tile[] allTitles = GameObject.FindObjectsOfType<Tile>();
        TitleOrganized = new List<Tile>();
        int CurrentPosition = 0;
        while (TitleOrganized.Count < allTitles.Length)
        {
            for (int i = 0; i < allTitles.Length; i++)
            {
                if (allTitles[i].PositionInMap == CurrentPosition)
                {
                    TitleOrganized.Add(allTitles[i]);
                    break;
                }
            }

            CurrentPosition++;
        }

        //A good way to kill the main thread
        foreach (var tile in TitleOrganized)
        {
            tile.InitalizeTile();
            
            if (tile.TitleType == TitleType.Land || tile.TitleType == TitleType.Station  )
            {
                
                LandInstancePairedLandType.AddLandTileInstance((LandTitleInstance)tile.TileInstance);   
            }
        }

        foreach (var landType in LandInstancePairedLandType)
        {
            LandTypePairedCount.Add(landType.Key,landType.Value.Count);
        }
        CurrentPosition++;
    }

 


    public void PlayerLandedOnTile(Player player, int TilePosition)
    {
        //todo: whatever the fuck imma write imporve it
        Tile tile = TitleOrganized[TilePosition];
        if (tile.TitleType == TitleType.Land)
        {
            //It's land
            PlayerLandedOnLand(player, tile);
        }
        else if (tile.TitleType == TitleType.Prison)
        {
            PlayerLandedOnPrison(player, tile);
        }
        else if (tile.TitleType == TitleType.SpecialEvent)
        {
            
            PlayerLandedOnSpecialEvent(player, tile);
        }
        else if (tile.TitleType == TitleType.Station)
        {
            PlayerLandedOnStation(player, tile);
        }
    }

    private void PlayerLandedOnStation(Player player, Tile tile)
    {
        TileLand tileLand = (TileLand)tile;
        _gameManager.PlayerLandedOnStation(player, tileLand.LandTitleInstance);
    }

    private void PlayerLandedOnSpecialEvent(Player player, Tile tile)
    {
        TileEvent tileEvent = (TileEvent)tile;
        _gameManager.PlayerLandedOnEvent(player,tileEvent.TitleDefinition_Casted);
    }

    private void PlayerLandedOnPrison(Player player, Tile tile)
    {
    }

    private void PlayerLandedOnLand(Player player, Tile tile)
    {
        TileLand tileLand = (TileLand)tile;
        _gameManager.PlayerLandedOnLand(player, tileLand.LandTitleInstance);
    }

    private void OnLandUpgraded(LandTitleInstance tileInstance)
    {
        // LandTitleInstance landTitleInstance = (LandTitleInstance)tileInstance;
        if (tileInstance.CurrentRank==0)
        {
            tileInstance.TileLand.HouseMesh.gameObject.SetActive(false);
            return;
        }
        tileInstance.TileLand.HouseMesh.gameObject.SetActive(true);
        tileInstance.TileLand.HouseMesh.material.color =
            MatchSettings.SingletonInstance.LandColors[tileInstance.CurrentRank-1];
        // MatchSettings.SingletonInstance.LandColors
        // landTitleInstance.TileLand.HouseMesh.material.color=Color.black;
        
    }
    private void LandMortgaged(LandTitleInstance landTitleInstance)
    {
        landTitleInstance.TileLand.SetTileMatColor(MatchSettings.SingletonInstance.MortgageColor);
    }
    private void OnLandRedeem(LandTitleInstance landTitleInstance)
    {
        landTitleInstance.TileLand.SetTileMatColor(MatchSettings.SingletonInstance.HighlightColor);
    }
}