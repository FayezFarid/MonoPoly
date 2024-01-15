using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OfflinePlayerItem : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button add;
    [SerializeField] private Button remove;

    private Action<OfflinePlayerItem> removePlayer;

    public string PlayerName
    {
        get => _inputField.text;
    }

    public void init(UnityAction addPlayer,Action<OfflinePlayerItem> removePlayer)
    {
        remove.onClick.RemoveAllListeners();
        add.onClick.RemoveAllListeners();
        if (addPlayer != null)
        {
            add.onClick.AddListener(addPlayer);
            
        }

        if (removePlayer != null)
        {
            this.removePlayer = removePlayer;
            remove.onClick.AddListener(RemovePlayerWrapper);
        }

        // Debug.Log($"Adding new item addPlayer null {addPlayer != null}  removePlayer null = {removePlayer != null}");
            add.gameObject.SetActive(addPlayer != null);
            remove.gameObject.SetActive(removePlayer != null);

    }
    public void SetButtonVisibilty(bool addActive, bool removeActive)
    {
        add.gameObject.SetActive(addActive);
        remove.gameObject.SetActive(removeActive);
    }

    private void RemovePlayerWrapper()
    {
        removePlayer.Invoke(this);
    }
    public void OnDestroy()
    {
        add.onClick.RemoveAllListeners();
        remove.onClick.RemoveAllListeners();
    }
}
