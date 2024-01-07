using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

//TODO: Add static
[CreateAssetMenu(menuName = "Monopoly/MatchSettings")]
public class MatchSettings : ScriptableObject
{
    #region Singleton

    private const string MATCHSETTING_SETTING = "DefaultMatchSettings";

    private static MatchSettings singletonMatchSettings;

    public static MatchSettings SingletonInstance
    {
        get
        {
            if (singletonMatchSettings == null)
            {
                UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<MatchSettings> op =
                    Addressables.LoadAssetAsync<MatchSettings>(MATCHSETTING_SETTING);
                // requires Addressables 1.17+
                singletonMatchSettings = op.WaitForCompletion();
            }

            return singletonMatchSettings;
        }
    }

    #endregion

    [Header("Colors")] public Color32 HighlightColor;
    public Color32 DeHighlightColor;

    public Color32 MortgageColor;

    public Color32[] PlayerPossibleColors;

    public Color32[] LandColors;

    public Color DefaultColor;
    public Color PlayerLostColor;
    public Color PlayerTurnColor;


    [Header("prefabs")] [SerializeField] private SmallPlayerInfoCard playerInfoCard;
    [SerializeField] private GameObject dicePrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private MoneyPopCard moneyCardPopUpPrefab;

    /// <summary>
    /// Mini player prefab
    /// </summary>
    public SmallPlayerInfoCard PlayerInfoCard => playerInfoCard;

    public GameObject PlayerPrefab => playerPrefab;

    public MoneyPopCard MoneyCardPopUpPrefab => moneyCardPopUpPrefab;

    public GameObject DicePrefab => dicePrefab;

    [Header("Values")] [SerializeField] private int initalMoney = 2000;
    [SerializeField] private int wholeLoopMoney = 200;
    [SerializeField] private int prisonFee;


    [SerializeField] private int playersNumber;
    [SerializeField] private int numberOfDices = 2;
    [SerializeField] private float diceRollTime;


    public int WholeLoopMoney => wholeLoopMoney;
    public int PrisonFee => prisonFee;
    public float DiceRollTime => diceRollTime;
    public int InitalMoney => initalMoney;
    public int PlayersNumber => playersNumber;
    public float PlayerMovementSpeed;
    [Range(0, 1)] public float BidIncreasePercentage;
    public int NumberOfDices => numberOfDices;


    [Header("Delays")] public float DelayAfterClosingMenu;
    public float DelayMoneyPopup = 2;
    public float DelayToCloseEventDescription = 2;

    public bool UseAppropriateColors;
    [Header("AI")] public float DelayBetweenAction;
}
