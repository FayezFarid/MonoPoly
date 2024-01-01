using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dice
{
    public int sides;
    public int rollValues;
#if UNITY_EDITOR
    public int FixedDiceValue;
#endif

    public Dice(int sides)
    {
        this.sides = sides;
    }

    // public void Roll()
    // {
    //     rollValues = UnityEngine.Random.Range(1, sides + 1);
    // }
    public int Roll()
    {
#if UNITY_EDITOR
        if (FixedDiceValue > 0)
            return FixedDiceValue;
#endif
        return UnityEngine.Random.Range(1, sides + 1);
    }
}