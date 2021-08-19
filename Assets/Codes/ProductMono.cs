using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductMono : MonoBehaviour
{

    public Text ProductName;
    public Text Description;
    public Text Price;

    public void Initialize(string productName, string description, int price)
    {
        if (productName.Length > 0)
            ProductName.text = productName;
        else
            ProductName.text = "未公開";

        if (description.Length > 0)
            Description.text = description;
        else
            Description.text = "無詳細資訊";

        if (price >= 0)
            Price.text = "$" + price.ToString("N0");
        else
            Price.text = "未販售";
    }
}
