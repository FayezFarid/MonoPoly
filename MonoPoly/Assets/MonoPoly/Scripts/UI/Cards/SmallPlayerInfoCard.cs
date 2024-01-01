using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SmallPlayerInfoCard : CardBase
{
    public struct SmallPlayerInfoCardHandle
    {
        public string playerName;
        public Color32 playerColor;

        public void Init(Player player)
        {
            playerColor = player.PlayerColor;
            playerName = player.PlayerName;
        }
        //Can't be here or it would also init it with wrong color. too lazy to make it work
        // public Color32 BaseImageColor;
    }
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Image colorImage;
    [SerializeField] private Button clickableButton;
    [SerializeField] private Image ParentImage;
    
    public Button ClickableButton => clickableButton;

    public override void Init<T>(InitalizationHandle<T> initalizationHandle)
    {
        SmallPlayerInfoCardHandle player=(SmallPlayerInfoCardHandle)(initalizationHandle.Data as object);
       Init(player.playerName, player.playerColor);
    }
    private void Init(string playerName, Color32 playerColor)
    {
        this.playerName.text = playerName;
        colorImage.color = playerColor;
    }

    public void ChangeParentImageColor(Color32 BaseImageColor)=> ParentImage.color = BaseImageColor;

    public void DisableButton()
    {
        if (clickableButton == null)
        {
            SpicyHarissaLogger.LogErrorCritical("SmallPlayerInfoCard::DisableButton was called but clickable button is null");
            return;
        }

        clickableButton.interactable = false;
        clickableButton.GetComponent<Image>().color = new Color(0,0,0,180);
    }

   
}
