using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Exhibition2Booth : MonoBehaviour {

    public GameObject Mask;
    [Header("Exhibition2Booth")]
    public Text Title;
    public Text DateRange;
    public Text City;
    public Text Popularity;

    private int index;
    private Exhibition exhibition;

    public void Initialize(int index, Exhibition exhibition)
    {
        this.index = index;
        this.exhibition = exhibition;
        Title.text = exhibition.DisplayName;
        DateRange.text = exhibition.StartDate.ToString("M/d") + " - " + exhibition.EndDate.ToString("M/d");
        if (exhibition.Location.Length >= 3)
            City.text = exhibition.Location.Substring(0, 3);
        else
            City.text = exhibition.Location;
        Popularity.text = exhibition.Popularity.ToString("N0");
    }

    public void OnButtonClikced()
    {
        BoothManager.Singleton.OnSearchBoothClicked(exhibition);
    }
}
