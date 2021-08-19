using EES.ClientAPIs;
using EES.ClientAPIs.ClientModules;
using EES.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExhibitionManager : MonoBehaviour {

    private static ExhibitionManager instance;
    public static ExhibitionManager Singleton
    {
        get
        {
            return instance;
        }
    }

    // The exhibition that you follow
    private const string MyExhibitionsFileName = "MyExhibitions";
    // All exhibition cached
    private const string ExhibitionsFileName = "Exhibitions";
    // Current Exhibition
    private const string CurrentExhibitionFileName = "CurrentExhibition";
    public Transform ExhibitionRoot;
    public Transform ExhibitionBoothRoot;
    public GameObject ExhibitionPrefab;
    public GameObject Exhibition2BoothPrefab;
    public GameObject NoExhibition;
    public GameObject NoExhibitionLike;
    public GameObject ExhibitionDetails;

    private List<Exhibition> myExhibition = new List<Exhibition>();
    private List<Exhibition> exhibitionsList = new List<Exhibition>();
    private Exhibition currentExhibition;

    private bool isInTheExhibition;
    public bool IsInTheExhibition { get { return isInTheExhibition; } }

    public void Initialize()
    {
        PopUp.Singleton.ShowLoading();
        NoExhibition.SetActive(false);
        EESAPI.GetExhibtion(new GeneralRequest(),
            (response) =>
            {
                foreach (EESExhibition EESexhibition in response.data)
                    if (EESexhibition.exhibitionId != Utilities.INVALID &&
                    EESexhibition.organizerId != Utilities.INVALID)
                        exhibitionsList.Add(new Exhibition(
                        EESexhibition.exhibitionId,
                        EESexhibition.organizerId,
                        EESexhibition.displayName,
                        EESexhibition.location,
                        EESexhibition.startDate,
                        EESexhibition.endDate,
                        EESexhibition.description,
                        EESexhibition.popularity));
                Utilities.DeleteAllChildGameObject(ExhibitionRoot);
                int i = 0;
                foreach (Exhibition exhibition in exhibitionsList)
                {
                    GameObject newGrid = Instantiate(ExhibitionPrefab);
                    newGrid.GetComponent<ExhibitionMono>().Initialize(i, exhibition, ExhibitionDetails);
                    Utilities.SetParentAndNormalize(newGrid.transform, ExhibitionRoot);
                    i++;
                }
                FileManager.SaveFile(ExhibitionsFileName, exhibitionsList);
                PopUp.Singleton.CloseLoading();
            },
            (error) =>
            {
                exhibitionsList = FileManager.LoadFile<List<Exhibition>>(ExhibitionsFileName);
                if (exhibitionsList == null)
                    exhibitionsList = new List<Exhibition>();
                if(exhibitionsList.Count > 0)
                {
                    Utilities.DeleteAllChildGameObject(ExhibitionRoot);
                    int i = 0;
                    foreach (Exhibition exhibition in exhibitionsList)
                    {
                        GameObject newGrid = Instantiate(ExhibitionPrefab);
                        newGrid.GetComponent<ExhibitionMono>().Initialize(i, exhibition, ExhibitionDetails);
                        Utilities.SetParentAndNormalize(newGrid.transform, ExhibitionRoot);
                        i++;
                    }
                }
                else
                {
                    if (error == CommonError.Rejected)
                        PopUp.Singleton.ShowError("伺服器拒絕存取\r\n即將關閉程式", false, () => { Application.Quit(); });
                    else if (error == CommonError.Timeout)
                    {
                        PopUp.Singleton.ShowError("連線逾時", true, () => { Initialize(); });
                    }
                    else if (error == CommonError.Unknown)
                    {
                        PopUp.Singleton.ShowError("未知錯誤", false);
                    }
                    NoExhibition.SetActive(true);
                }
                PopUp.Singleton.CloseLoading();   
            }
        ); 
    }

    public bool CheckTheExhibitionIsFollow(Exhibition exhibition)
    {
        foreach(Exhibition e in myExhibition)
        {
            if (e.ExhibitionId == exhibition.ExhibitionId)
                return true;
        }
        return false;
    }

    public void AddFolllowExhibition(Exhibition exhibition)
    {
        if (exhibition.IsValid())
            myExhibition.Add(exhibition);
        FileManager.SaveFile(MyExhibitionsFileName, myExhibition);
        RefreshFollowedExhibition();
    }


    public void RemoveFolllowExhibition(Exhibition exhibition)
    {
        int removeIndex = Utilities.INVALID;
        int i = 0;
        foreach (Exhibition e in myExhibition)
        {
            if (e.ExhibitionId == exhibition.ExhibitionId)
                removeIndex = i;
            i++;
        }
        myExhibition.RemoveAt(removeIndex);
        FileManager.SaveFile(MyExhibitionsFileName, myExhibition);
        RefreshFollowedExhibition();
    }

    private void RefreshFollowedExhibition()
    {
        Utilities.DeleteAllChildGameObject(ExhibitionBoothRoot);
        int i = 0;
        foreach (Exhibition exhibition in myExhibition)
        {
            GameObject newGrid = Instantiate(Exhibition2BoothPrefab);
            newGrid.GetComponent<Exhibition2Booth>().Initialize(i, exhibition);
            Utilities.SetParentAndNormalize(newGrid.transform, ExhibitionBoothRoot);
            i++;
        }
        if(i == 0)
            NoExhibitionLike.SetActive(true);
        else
            NoExhibitionLike.SetActive(false);
    }

    public void RefreshBooths()
    {
        BoothManager.Singleton.OnSearchBoothClicked(currentExhibition);
    }

    public void EnterExhibition(Exhibition exhibition)
    {
        currentExhibition = exhibition;
        isInTheExhibition = true;
        FileManager.SaveFile(CurrentExhibitionFileName, currentExhibition);
    }

    public void LeaveExhibition()
    {
        currentExhibition = null;
        isInTheExhibition = false;
        FileManager.SaveFile(CurrentExhibitionFileName, currentExhibition);
    }

    private void Start()
    {
        instance = this;
        myExhibition = FileManager.LoadFile<List<Exhibition>>(MyExhibitionsFileName);
        currentExhibition = FileManager.LoadFile<Exhibition>(CurrentExhibitionFileName);
        exhibitionsList = new List<Exhibition>();
        if (myExhibition == null)
        {
            Utilities.DeleteAllChildGameObject(ExhibitionBoothRoot);
            NoExhibitionLike.SetActive(true);
            myExhibition = new List<Exhibition>();
        }
        else
        {
            RefreshFollowedExhibition();
        }

        if (currentExhibition == null)
            isInTheExhibition = false;
        else
            isInTheExhibition = true;
    }
}
