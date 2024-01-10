using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monopoly/ Event Effect Definition/Relative position")]
public class EventEffectTelportRelative : EventEffectDefinition
{
    [SerializeField] private bool forward;
    [SerializeField] private int stepsToTake;

    public override List<Action> GetActions(GameManager gameManager, Action onEnd)
    {
        Action action = forward
            ? () =>
            {
                MoveForward(gameManager);
                onEnd.Invoke();
            }
            : () =>
            {
                MoveBackward(gameManager);
                onEnd.Invoke();
            };
        return new List<Action>() { action };
    }

    private void MoveForward(GameManager gameManager)
    {
        gameManager.StartCoroutine(gameManager.PlacePlayerAnimated(gameManager.CurrentPlayer,
            gameManager.CurrentPlayer.CurrentPosition + stepsToTake, stepsToTake));
    }

    private void MoveBackward(GameManager gameManager)
    {
        gameManager.StartCoroutine(gameManager.PlacePlayerReverseAnimated(gameManager.CurrentPlayer,
            gameManager.CurrentPlayer.CurrentPosition - stepsToTake, stepsToTake));
    }
}