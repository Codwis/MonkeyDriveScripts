using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarehouseObject : HoldableObject
{
    [Header("Warehouse Object")]
    public int ShelfNum;
    public TMP_Text BoxText;
    public override void Start()
    {
        base.Start();
        BoxText.text = IntToRoman(ShelfNum);
    }

    private string IntToRoman(int num)
    {
        // Define the Roman numeral values and their corresponding symbols
        int[] values = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
        string[] symbols = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

        string result = string.Empty;

        // Loop through each value and symbol
        for (int i = 0; i < values.Length && num > 0; i++)
        {
            // Repeat the symbol while the number is greater or equal to the value
            while (num >= values[i])
            {
                result += symbols[i];
                num -= values[i];
            }
        }

        return result;
    }
}
