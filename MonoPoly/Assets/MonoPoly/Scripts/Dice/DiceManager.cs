using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceManager : MonoBehaviour
{
    public static Quaternion[] StaticDiceRotation = new Quaternion[6]
    {
        new Quaternion(-0.6f, 0, 0, 0.7f),
        Quaternion.identity,
        new Quaternion(-0.005f, 0.001f, -0.7f, 0.7f),
        new Quaternion(0, 0, 0.68f, 0.72f),
        new Quaternion(1, 0, 0, 0),
        new Quaternion(0.707f, 0, 0, 0.707f),
    };

    public bool FixValue { get; set; } = false;
    public int FixedValue { get; set; }

    [SerializeField] private Vector3 DiceSpawnStartPoint;
    [SerializeField] private Vector3 DiceSpacing;

    [SerializeField] private int sides = 6;
    private Transform[] _dices;
    private Action<int, bool> _onDiceRollCompleted;

    public Action<int, bool> OnDiceRollCompleted
    {
        get => _onDiceRollCompleted;
        set => _onDiceRollCompleted = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="DiceNumber"></param>
    /// <param name="onDiceRollCompleted"> Int parameter is final value with the sum</param>
    public void Init(int DiceNumber, Action<int, bool> onDiceRollCompleted)
    {
        _onDiceRollCompleted = onDiceRollCompleted;
        _dices = new Transform[DiceNumber];
        for (int i = 0; i < DiceNumber; i++)
        {
            _dices[i] = Instantiate(MatchSettings.SingletonInstance.DicePrefab).transform;
        }

        Vector3 AccumaltedPosition = DiceSpawnStartPoint;
        // _dices[0].position = AccumaltedPosition;

        for (int i = 0; i < _dices.Length; i++)
        {
            _dices[i].position = AccumaltedPosition;
            AccumaltedPosition += DiceSpacing;
        }
    }

    public void RollDice()
    {
        int[] expectedValue = new int[_dices.Length];
#if UNITY_EDITOR
        if (FixValue)
        {
            for (int i = 0; i < expectedValue.Length; i++)
            {
                expectedValue[i] = FixedValue;
            }

            StartCoroutine(rollDiceCorotuine(expectedValue));
            return;
        }
#endif
        for (int i = 0; i < expectedValue.Length; i++)
        {
            expectedValue[i] = UnityEngine.Random.Range(1, sides + 1);
        }

        StartCoroutine(rollDiceCorotuine(expectedValue));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="expectedValue">Must match Dices Number and must be in between 1-6</param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    private IEnumerator rollDiceCorotuine(int[] expectedValue)
    {
        if (expectedValue.Length != _dices.Length)
        {
            throw new IndexOutOfRangeException("Expected Value does not match dices number");
        }

        float CurrentTime = 0;
        float Angle = 180;
        while (CurrentTime < MatchSettings.SingletonInstance.DiceRollTime)
        {
            for (int i = 0; i < _dices.Length; i++)
            {
                int x = Random.Range(0, 2);
                int y = Random.Range(0, 2);
                int z = Random.Range(0, 2);

                _dices[i].Rotate(new Vector3(x, y, z), Angle);
            }

            yield return null;
            CurrentTime += Time.deltaTime;
            Angle -= Angle * 0.01f * Time.deltaTime;
        }


        int TotalRollValue = 0;
        for (int i = 0; i < _dices.Length; i++)
        {
            _dices[i].rotation = StaticDiceRotation[expectedValue[i] - 1];
            TotalRollValue += expectedValue[i];

            SpicyHarissaLogger.Log($"I [{i}] Value [{expectedValue[i]}] ", LogLevel.Verbose,
                SpicyHarissaLogger.DICE_DEBUG_KEY);

            // _dices[i].Rotate(Vector3.forward,-90);
        }

        yield return new WaitForSeconds(1f);


        _onDiceRollCompleted?.Invoke(TotalRollValue, AreDicesEqual(expectedValue));

        //Cleanup
    }

    private bool AreDicesEqual(int[] expectedValue)
    {
        bool AreEquals = true;
        int FirstValue = expectedValue[0];
        for (int i = 1; i < expectedValue.Length; i++)
        {
            if (FirstValue != expectedValue[i])
            {
                return false;
            }
        }

        return AreEquals;
    }
}