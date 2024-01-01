using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyPopCard : CardBase
{
    [SerializeField]  private TextMeshProUGUI text;
    public struct MoneyPopHandle
    {
        public int MoneyInput;
    }
    public override void Init<T>(InitalizationHandle<T> initalizationHandle)
    {
        int number =(int)( initalizationHandle.Data as object);

        string FinalString = string.Empty;
        if (number > 0)
        {
            FinalString = "+";
            text.color=Color.green;
        }
        else
        {
            // FinalString = "-";
            text.color = Color.red;
        }

        FinalString += number.ToString();
        text.text =FinalString;
    }
}
