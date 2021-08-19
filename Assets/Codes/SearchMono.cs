using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchMono : MonoBehaviour {

    private int index;
    private string tagName;

    public Text History;

    public void Initialize(int index, string tagName)
    {
        this.index = index;
        this.tagName = tagName;
        History.text = tagName;
    }

    public void OnSearchClicked()
    {
        SearchManager.Singleton.OnSearch(tagName);
    }
}
