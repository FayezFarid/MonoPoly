using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Monopoly/ Event Effect Definition/Houses Related")]
public class EventEffectHouseGainLose : EventEffectDefinition
{
    [SerializeField] private bool RemoveHouses;
    [SerializeField] private int numberOfHousesValue;

    public override List<Action> GetActions(GameManager gameManager, Action onEnd)
    {
        if (!gameManager.PlayerOwnsLandSet(gameManager.CurrentPlayer))
            return null;

        Action action = () =>
        {
            UpgradePlayerLand(gameManager);
            onEnd();
        };
        return new List<Action>() { action };
    }

    private void UpgradePlayerLand(GameManager gameManager)
    {
        List<TileLand> tileLands = gameManager.GetPlayerOwnedLandInstanceSet(gameManager.CurrentPlayer);
        for (int i = 0; i < numberOfHousesValue; i++)
        {
            int RandomIndex = Random.Range(0, tileLands.Count);

            if (!RemoveHouses)
                tileLands[RandomIndex].LandTitleInstance.UpgradeLand();
            else
                tileLands[RandomIndex].LandTitleInstance.DownGradeLand();
        }
    }
}