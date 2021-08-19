using EES.ClientAPIs.ClientModules;
using EES.Utilities;
using System.Collections;
using System.Collections.Generic;

public class Product
{
    private int productId = Utilities.INVALID;
    private int holderId = Utilities.INVALID;
    private string holderName;
    private string productName;
    private string description;
    private int price;

    public string HolderName
    {
        get { return holderName; }
    }
    public string ProductName
    {
        get { return productName;  }
    }
    public string Description
    {
        get { return description; }
    }
    public int Price
    {
        get { return price; }
    }

    public Product(int productId, int holderId, string holderName, string productName, string description, int price)
    {
        if(productId != Utilities.INVALID && holderId != Utilities.INVALID)
        {
            this.productId = productId;
            this.holderId = holderId;
            this.holderName = holderName;
            this.productName = productName;
            this.description = description;
            this.price = price;
        }
    }

    public static Product Convert(EESProduct eesProduct)
    {
        if (eesProduct.productId != Utilities.INVALID &&
            eesProduct.holderId != Utilities.INVALID &&
            eesProduct.productName.Length > 0 &&
            eesProduct.price >= 0)
        {
            return new Product(eesProduct.productId, eesProduct.holderId, eesProduct.holderName, eesProduct.productName, eesProduct.description,
                eesProduct.price);
        }
        else
        {
            return null;
        }
    }
}
