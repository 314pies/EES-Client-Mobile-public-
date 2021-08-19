using EES.Utilities;
using FlipWebApps.BeautifulTransitions.Scripts.Transitions.Components.GameObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ExhibitionMono : MonoBehaviour
{

    public GameObject ExhibitionDetails;
    public GameObject Mask;
    [Header("Exhibition")]
    public Text Title;
    public Text DateRange;
    public Text City;
    public Text Popularity;

    private int index;
    private Exhibition exhibition;

    public void Initialize(int index, Exhibition exhibition, GameObject exhibitionDetails)
    {
        StartCoroutine(CloseMask());
        this.index = index;
        this.exhibition = exhibition;
        Title.text = exhibition.DisplayName;
        DateRange.text = exhibition.StartDate.ToString("M/d") + " - " + exhibition.EndDate.ToString("M/d");
        if (exhibition.Location.Length >= 3)
            City.text = exhibition.Location.Substring(0, 3);
        else
            City.text = exhibition.Location;
        Popularity.text = exhibition.Popularity.ToString("N0");
        ExhibitionDetails = exhibitionDetails;
    }

    IEnumerator CloseMask()
    {
        yield return new WaitForSeconds(0.2f);
        Utilities.TransitionOut(Mask);
    }

    public void OnExhibitionClicked()
    {
        if (ExhibitionDetails.GetComponent<ExhibitionInfo>().SetExhibitionInfo(exhibition))
            Utilities.Transition(ExhibitionDetails);
        else
            PopUp.Singleton.ShowError("展覽資訊設定錯誤", false);
    }
}
