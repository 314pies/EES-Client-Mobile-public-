using EES.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ExhibitionInfo : MonoBehaviour
{

    public Sprite BorderStar;
    public Sprite FullStar;
    public Color Green;
    public Color Red;
    [Header("Details")]
    public Text DisplayName;
    public Text Location;
    public Text StartDate;
    public Text EndDate;
    public Text Description;
    public Text View;
    public Image Star;
    public Text FollowText;

    private Exhibition exhibition;
    private bool isFollow = false;

    public bool SetExhibitionInfo(Exhibition exhibition)
    {
        try
        {
            DisplayName.text = exhibition.DisplayName;
            Location.text = exhibition.Location;
            StartDate.text = "開始：" + exhibition.StartDate.ToString("yyyy 年 M 月 d 日 (dddd)",
                CultureInfo.CreateSpecificCulture("zh-TW"));
            EndDate.text = "結束：" + exhibition.EndDate.ToString("yyyy 年 M 月 d 日 (dddd)",
                CultureInfo.CreateSpecificCulture("zh-TW"));
            Description.text = exhibition.Description;
            View.text = "共有 " + exhibition.Popularity.ToString("N0") + " 人次瀏覽";
            this.exhibition = exhibition;
            if (ExhibitionManager.Singleton.CheckTheExhibitionIsFollow(exhibition))
                isFollow = true;
            else
                isFollow = false;
            setFollow();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
        return true;
    }

    private void setFollow()
    {
        if (isFollow)
        {
            Star.sprite = FullStar;
            FollowText.text = "取消關注";
            Star.color = Red;
            FollowText.color = Red;
        }
        else
        {
            Star.sprite = BorderStar;
            FollowText.text = "關注展覽";
            Star.color = Green;
            FollowText.color = Green;
        }
    }

    public void OnMapClicked()
    {
        Application.OpenURL("https://www.google.com.tw/maps?q=" + exhibition.Location);
    }

    public void OnStarClicked()
    {
        if (AccountManager.Singleton.CurrentAccountStatus != AccountManager.AccountType.Member)
        {
            PopUp.Singleton.ShowAsking("您尚未登入本系統\r\n無法使用此功能，要登入嗎？",
                null,
                () => { AccountManager.Singleton.RejectToLogin(); });
            return;
        }
            
        isFollow = !isFollow;
        if (isFollow)
        {
            ExhibitionManager.Singleton.AddFolllowExhibition(exhibition);
        }
        else
        {
            ExhibitionManager.Singleton.RemoveFolllowExhibition(exhibition);
        }
        setFollow();
    }

    public void OnBackClicked()
    {
        Utilities.TransitionOut(gameObject);
    }
}
