using EES.ClientAPIs;
using EES.ClientAPIs.ClientModules;
using EES.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoothManager : MonoBehaviour
{

    private const string BoothsFileName = "Booths";
    private static BoothManager instance;
    public static BoothManager Singleton
    {
        get
        {
            return instance;
        }
    }

    public GameObject BoothsListPanel;
    public GameObject BoothDetails;
    public Transform BoothsRoot;
    public GameObject BoothPrefab;
    public Text title;
    public GameObject NoBooths;

    private List<Booth> boothsList = new List<Booth>();

    void Start()
    {
        instance = this;
        boothsList = FileManager.LoadFile<List<Booth>>(BoothsFileName);
        if(boothsList != null)
        {
            Utilities.DeleteAllChildGameObject(BoothsRoot);
            int i = 0;
            foreach (Booth booth in boothsList)
            {
                GameObject newGrid = Instantiate(BoothPrefab);
                newGrid.GetComponent<BoothMono>().Initialize(i, booth, BoothDetails);
                Utilities.SetParentAndNormalize(newGrid.transform, BoothsRoot);
                i++;
            }
        }
        gameObject.SetActive(false);
    }

    public void OnSearchBoothClicked(Exhibition exhibition)
    {
        ExhibitionManager.Singleton.EnterExhibition(exhibition);
        title.text = exhibition.DisplayName;
        PopUp.Singleton.ShowLoading();
        Utilities.Transition(gameObject);
        NoBooths.SetActive(false);
        if(exhibition.ExhibitionId != Utilities.INVALID)
        {
            EESAPI.GetBooth(new GetBoothRequest(exhibition.ExhibitionId),
                (response) =>
                {
                    Utilities.DeleteAllChildGameObject(BoothsRoot);
                    int i = 0;
                    foreach(EESBooth eesBooth in response.data)
                    {
                        Booth booth = Booth.Convert(eesBooth);
                        if (booth != null)
                        {
                            GameObject newGrid = Instantiate(BoothPrefab);
                            newGrid.GetComponent<BoothMono>().Initialize(i, booth, BoothDetails);
                            Utilities.SetParentAndNormalize(newGrid.transform, BoothsRoot);
                            i++;
                        }
                    }
                    if(i != 0)
                        NoBooths.SetActive(false);
                    PopUp.Singleton.CloseLoading();
                    Utilities.Transition(BoothsListPanel);
                },
                (error) =>
                {
                    if (error == CommonError.Rejected)
                        PopUp.Singleton.ShowError("伺服器拒絕存取\r\n即將關閉程式", false, () => { Application.Quit(); });
                    else if (error == CommonError.Timeout)
                    {
                        PopUp.Singleton.ShowError("連線逾時", true, () => { OnSearchBoothClicked(exhibition); });
                    }
                    else if (error == CommonError.Unknown)
                    {
                        PopUp.Singleton.ShowError("未知錯誤", false);
                    }
                    NoBooths.SetActive(true);
                    PopUp.Singleton.CloseLoading();
                }
            );
        }
        else
        {
            PopUp.Singleton.ShowError("展覽ID錯誤", false);
        }
    }

    public void OnBackClicked()
    {
        ExhibitionManager.Singleton.LeaveExhibition();
        Utilities.TransitionOut(BoothsListPanel);
    }
}
