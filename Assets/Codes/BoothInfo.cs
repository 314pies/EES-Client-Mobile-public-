using EES.ClientAPIs;
using EES.ClientAPIs.ClientModules;
using EES.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoothInfo : MonoBehaviour
{

    public Sprite BorderStar;
    public Sprite FullStar;
    public Color Green;
    public Color Red;
    [Header("Details")]
    public Text DisplayName;
    public Text HolderName;
    public Text Position;
    public Text View;
    public Image Star;
    public Text FollowText;

    public Transform ProductRoot;
    public GameObject ProductPrefab;
    public GameObject NoProducts;

    private Booth booth;
    private bool isFollow = false;

    public bool SetBoothInfo(Booth booth)
    {

        DisplayName.text = booth.DisplayName;
        HolderName.text = booth.HolderName;
        Position.text = booth.BoothPosition;
        View.text = booth.Popularity.ToString("N0") + " 次瀏覽";
        this.booth = booth;
        if (SubscribeManager.Singleton.CheckSubscribe(booth.HolderId))
            isFollow = true;
        else
            isFollow = false;
        setFollow();

        try
        {
            DisplayProduct();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
        return true;
    }

    public void DisplayProduct()
    {
        if (booth.BoothId != Utilities.INVALID)
        {
            PopUp.Singleton.ShowLoading();
            Utilities.DeleteAllChildGameObject(ProductRoot);
            EESAPI.GetProduct(new GetProductRequest(booth.BoothId),
                (response) =>
                {
                    int i = 0;
                    foreach (EESProduct eesProduct in response.data)
                    {
                        Product product = Product.Convert(eesProduct);
                        if (product != null)
                        {
                            GameObject newGrid = Instantiate(ProductPrefab);
                            newGrid.GetComponent<ProductMono>().Initialize(product.ProductName, product.Description, product.Price);
                            Utilities.SetParentAndNormalize(newGrid.transform, ProductRoot);
                            i++;
                        }
                    }
                    PopUp.Singleton.CloseLoading();
                },
                (error) =>
                {
                    if (error == CommonError.Rejected)
                        PopUp.Singleton.ShowError("伺服器拒絕存取\r\n即將關閉程式", false, () => { Application.Quit(); });
                    else if (error == CommonError.Timeout)
                    {
                        PopUp.Singleton.ShowError("連線逾時", true, () => { DisplayProduct(); });
                    }
                    else if (error == CommonError.Unknown)
                    {
                        PopUp.Singleton.ShowError("未知錯誤", false);
                    }
                    NoProducts.SetActive(true);
                    PopUp.Singleton.CloseLoading();
                }
            );
        }
        else
        {
            PopUp.Singleton.ShowError("攤位ID錯誤", false);
        }
    }

    private void setFollow()
    {
        if (isFollow)
        {
            Star.sprite = FullStar;
            FollowText.text = "取消追蹤";
            Star.color = Red;
            FollowText.color = Red;
        }
        else
        {
            Star.sprite = BorderStar;
            FollowText.text = "追蹤用戶";
            Star.color = Green;
            FollowText.color = Green;
        }
    }

    public void OnStarClicked()
    {
        isFollow = !isFollow;
        if (isFollow)
        {
            SubscribeManager.Singleton.AddSubscribe(new Subscribe(booth.HolderId, booth.HolderName));
        }
        else
        {
            SubscribeManager.Singleton.RemoveSubscribe(new Subscribe(booth.HolderId, booth.HolderName));
        }
        setFollow();
    }

    public void OnBackClicked()
    {
        Utilities.TransitionOut(gameObject);
    }
}
