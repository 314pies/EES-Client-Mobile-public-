using EES.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private static UIManager instance;
    public static UIManager Singleton
    {
        get
        {
            return instance;
        }
    }

    public GameObject Exhibition;
    public GameObject Login;
    public GameObject LoginPanel;
    public GameObject Register;
    public GameObject Error;
    public GameObject Info;
    public GameObject Ticket;
    
    public GameObject Scan;
    public GameObject Loading;
    public GameObject ExhibitionInfo;
    public GameObject Booth;
    public GameObject BoothsPanel;
    public GameObject ProductPanel;
    public GameObject Search;
    public GameObject Subscripe;

    public Button[] BannerButton;
    public Image[] BannerImage;
    public Text[] BannerText;

    public Color Selected;
    public Color UnSelected;
    public Color Rejected;

    void Awake()
    {
        instance = this;
        Login.SetActive(true);
        Exhibition.SetActive(true);
        ExhibitionInfo.SetActive(false);
        Booth.SetActive(true);
        BoothsPanel.SetActive(false);
        ProductPanel.SetActive(false);
        Search.SetActive(true);
        Subscripe.SetActive(true);
    }

    public void OnExhibitionClicked()
    {
        Exhibition.SetActive(true);
        ExhibitionInfo.SetActive(false);
        Booth.SetActive(false);
        BoothsPanel.SetActive(false);
        ProductPanel.SetActive(false);
        Search.SetActive(false);
        Subscripe.SetActive(false);
        SetUIColors(0);
    }

    public void OnBoothClicked()
    {
        Booth.SetActive(true);
        ExhibitionInfo.SetActive(false);
        ProductPanel.SetActive(false);
        if (ExhibitionManager.Singleton.IsInTheExhibition)
        {
            ExhibitionManager.Singleton.RefreshBooths();
        }
        else
        {
            BoothsPanel.SetActive(false);
        }
        Search.SetActive(false);
        Subscripe.SetActive(false);
        SetUIColors(1);
    }

    public void OnSearchClicked()
    {
        Search.SetActive(true);
        Subscripe.SetActive(false);
        SetUIColors(2);
    }

    public void OnSubClicked()
    {
        SubscribeManager.Singleton.DisplaySub(0);
        Search.SetActive(false);
        Subscripe.SetActive(true);
        SetUIColors(3);
    }

    public void SetAccountColors()
    {
        BannerButton[4].interactable = false;
        BannerImage[4].color = Rejected;
        BannerText[4].color = Rejected;
        if (AccountManager.Singleton.CurrentAccountStatus == AccountManager.AccountType.Guest)
        {
            BannerButton[1].interactable = false;
            BannerButton[3].interactable = false;
            BannerImage[1].color = Rejected;
            BannerImage[3].color = Rejected;
            BannerText[1].color = Rejected;
            BannerText[3].color = Rejected;
        }
        else
        {
            BannerButton[1].interactable = true;
            BannerButton[3].interactable = true;
            BannerImage[1].color = UnSelected;
            BannerImage[3].color = UnSelected;
            BannerText[1].color = UnSelected;
            BannerText[3].color = UnSelected;
        }
    }

    public void SetUIColors(int index)
    {
        if(index >= 0 && index < 4)
        {
            for(int i = 0; i < 4; i++)
            {
                if(index == i)
                {
                    BannerImage[i].color = Selected;
                    BannerText[i].color = Selected;
                }
                else
                {
                    if(AccountManager.Singleton.CurrentAccountStatus == AccountManager.AccountType.Guest)
                    {
                        if(i == 1)
                        {
                            BannerImage[1].color = Rejected;
                            BannerText[1].color = Rejected;
                        }
                        else if(i == 3)
                        {
                            BannerImage[3].color = Rejected;
                            BannerText[3].color = Rejected;
                        }
                        else
                        {
                            BannerImage[i].color = UnSelected;
                            BannerText[i].color = UnSelected;
                        }
                    }
                    else
                    {
                        BannerImage[i].color = UnSelected;
                        BannerText[i].color = UnSelected;
                    }
                }
            }
        }
    }
}
