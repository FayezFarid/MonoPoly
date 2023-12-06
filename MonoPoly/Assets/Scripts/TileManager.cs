using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private List<Tile> TitleOrganized;

    private GameManager _gameManager;

    public Tile this[int index]
    {
        get { return TitleOrganized[index]; }
    }

    public int TileCount => TitleOrganized.Count;

    public void InitalizeWithGameManager(GameManager gameManager)
    {
        _gameManager = GetComponent<GameManager>();
        // SortTitles();
    }

    public void SortTitles()
    {
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
        _gameManager.PlayerLanededOnEvent(player,tileEvent.TitleDefinition_Casted);
    }

    private void PlayerLandedOnPrison(Player player, Tile tile)
    {
    }

    private void PlayerLandedOnLand(Player player, Tile tile)
    {
        TileLand tileLand = (TileLand)tile;
        _gameManager.PlayerLandedOnLand(player, tileLand.LandTitleInstance);
    }
}