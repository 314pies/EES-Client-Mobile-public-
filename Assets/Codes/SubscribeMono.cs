using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubscribeMono : MonoBehaviour {

    private int index;
    private Subscribe subscribe;
    private bool iHaveSubscribed;
    public Text DisplayName;
    public Image Star;

    public Color UnSub;
    public Color Sub;

    public Sprite BorderStar;
    public Sprite FullStar;

    public void Initialize(int index, Subscribe subscribe, bool iHaveSubscribed)
    {
        this.index = index;
        this.subscribe = subscribe;
        this.iHaveSubscribed = iHaveSubscribed;
        setStar(iHaveSubscribed);
        DisplayName.text = subscribe.displayName;
    }

    private void setStar(bool iHaveSubscribed)
    {
        if (iHaveSubscribed)
        {
            Star.sprite = FullStar;
            Star.color = Sub;
        }
        else
        {
            Star.sprite = BorderStar;
            Star.color = UnSub;
        }
    }

    public void OnStarClicked()
    {
        if(iHaveSubscribed)
            SubscribeManager.Singleton.RemoveSubscribe(subscribe);
        else
            SubscribeManager.Singleton.AddSubscribe(subscribe);
        iHaveSubscribed = !iHaveSubscribed;
        setStar(iHaveSubscribed);
    }
}
