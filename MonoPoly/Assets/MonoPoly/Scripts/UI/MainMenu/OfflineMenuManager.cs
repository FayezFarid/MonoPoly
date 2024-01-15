using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class OfflineMenuManager : MonoBehaviour
{
    private static readonly int MaxPlayerCount = 8;
    private readonly string MaxPlayersReached = "Max player number has been reached";
    private const string GAMESCENE_ADD = "MainScene.unity";

    [SerializeField] private Transform content;
    [SerializeField] private GameObject playerItemPrefab;

    [SerializeField] private List<OfflinePlayerItem> existingPlayers = new List<OfflinePlayerItem>(MaxPlayerCount);

    //[SerializeField] private LoadScreen _loadingscreen;
    public void StartGame()
    {
        MatchSettings.SingletonInstance.RunTimePlayerDatas = new List<MatchSettings.RunTimePlayerData>();

        // foreach (var playerItem in existingPlayers)
        // {
        //     string PlayerName=
        //     MatchSettings.SingletonInstance.RunTimePlayerDatas.Add(new MatchSettings.RunTimePlayerData()
        //         { Name = playerItem.PlayerName });
        // }

        MatchSettings.RunTimePlayerData runTimePlayerData = new MatchSettings.RunTimePlayerData();
        for (int i = 0; i < existingPlayers.Count; i++)
        {
            runTimePlayerData.Name = string.IsNullOrEmpty(existingPlayers[i].PlayerName)
                ? $"Player {i + 1}"
                : existingPlayers[i].PlayerName;
            MatchSettings.SingletonInstance.RunTimePlayerDatas.Add(runTimePlayerData);
        }

        var op = Addressables.LoadSceneAsync(GAMESCENE_ADD);
    }

    private void Start()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            //optimisation done right :)
            var obj = content.transform.GetChild(i);
            existingPlayers.Add(obj.GetComponent<OfflinePlayerItem>());
        }

        if (existingPlayers.Count == 0)
        {
            Debug.LogError($"There is no initial player?");
        }
        else
        {
            existingPlayers[0].init(AddPlayer, null);
            existingPlayers[1].init(AddPlayer, null);
        }
    }

    private void AddPlayer()
    {
        if (existingPlayers.Count == MaxPlayerCount)
        {
            SpicyHarissaLogger.Log(MaxPlayersReached, LogLevel.Standard);
            return;
        }

        var gameobj = Instantiate(playerItemPrefab, content, false);
        var item = gameobj.GetComponent<OfflinePlayerItem>();
        existingPlayers.Add(item);
        //now adjust the rest
        PlayersChanged();
    }

    private void RemovePlayer(OfflinePlayerItem _item)
    {
        if (existingPlayers.Remove(_item))
        {
            Destroy(_item.gameObject);
            PlayersChanged();
        }
        else
        {
            Debug.LogError($"Could not remove item {_item?.PlayerName}");
        }
    }

    private void PlayersChanged()
    {
        UnityAction addplayer = null;
        Action<OfflinePlayerItem> removePlayer = null;
        for (int i = 0; i < existingPlayers.Count; i++)
        {
            if (i > 1)
            {
                removePlayer = RemovePlayer;
            }

            if (i != MaxPlayerCount - 1 && i == existingPlayers.Count - 1)
            {
                addplayer = AddPlayer;
            }

            existingPlayers[i].init(addplayer, removePlayer);
        }
    }
}