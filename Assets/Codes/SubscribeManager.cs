using EES.ClientAPIs;
using EES.ClientAPIs.ClientModules;
using EES.Utilities;
using FlipWebApps.BeautifulTransitions.Scripts.Transitions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubscribeManager : MonoBehaviour
{

    private const string MySubscribeFileName = "MySubscribes";
    private const string SubscribeMeFileName = "SubscribeMe";

    private static SubscribeManager instance;
    public static SubscribeManager Singleton
    {
        get
        {
            return instance;
        }
    }

    private enum SubMode
    {
        My,
        Me
    }

    private SubMode currentSubMode;
    private List<Subscribe> mySubscribesList;
    private List<Subscribe> subscribeMeList;

    public Color SelectedColor;
    public Transform MySubRoot;
    public Transform SubMeRoot;
    public GameObject SubPrefab;
    public GameObject NoMySub;
    public GameObject NoSubMe;
    public GameObject MovePanel;
    public Image MySubImage;
    public Image SubMeImage;
    public Text Caption;

    public void GetSubscribe()
    {
        PopUp.Singleton.ShowLoading();
        EESAPI.GetSubscribe(new GeneralRequest(),
            (response) =>
            {
                subscribeMeList = response.data.subscribers;
                mySubscribesList = response.data.subscribed;
                DisplaySub((int)currentSubMode);
                PopUp.Singleton.CloseLoading();
            },
            (error) =>
            {
                if (error == CommonError.Rejected)
                    PopUp.Singleton.ShowError("伺服器拒絕存取\r\n即將關閉程式", false, () => { Application.Quit(); });
                else if (error == CommonError.Timeout)
                {
                    PopUp.Singleton.ShowError("連線逾時", true, () => { GetSubscribe(); });
                }
                else if (error == CommonError.Unknown)
                {
                    PopUp.Singleton.ShowError("未知錯誤", false);
                }
                DisplaySub((int)currentSubMode);
                PopUp.Singleton.CloseLoading();
            }
        );
    }

    public bool CheckSubscribe(int accountId)
    {
        foreach (Subscribe s in mySubscribesList)
        {
            if (s.accountId == accountId)
                return true;
        }
        return false;
    }

    public void AddSubscribe(Subscribe subscribe)
    {
        if (subscribe.accountId != Utilities.INVALID)
        {
            bool contain = false;
            foreach (Subscribe s in mySubscribesList)
                if (subscribe.accountId == s.accountId)
                    contain = true;
            if (!contain)
            {
                PopUp.Singleton.ShowLoading();
                EESAPI.AddSubscribe(new SubscribeRequest(subscribe.accountId),
                    (response) =>
                    {
                        mySubscribesList.Clear();
                        foreach (EESUserData e in response.data)
                        {
                            mySubscribesList.Add(new Subscribe(e.accountId, e.displayName));
                        }
                        FileManager.SaveFile(MySubscribeFileName, mySubscribesList);
                        DisplaySub((int)currentSubMode);
                        PopUp.Singleton.CloseLoading();
                    },
                    (error) =>
                    {
                        if (error == CommonError.Rejected)
                            PopUp.Singleton.ShowError("伺服器拒絕存取\r\n即將關閉程式", false, () => { Application.Quit(); });
                        else if (error == CommonError.Timeout)
                        {
                            PopUp.Singleton.ShowError("連線逾時", true, () => { AddSubscribe(subscribe); });
                        }
                        else if (error == CommonError.DataInsertFailed)
                        {
                            PopUp.Singleton.ShowError("您已經有追蹤過該用戶", false);
                        }
                        else if (error == CommonError.Unknown)
                        {
                            PopUp.Singleton.ShowError("未知錯誤", false);
                        }
                        mySubscribesList.Add(subscribe);
                        FileManager.SaveFile(MySubscribeFileName, mySubscribesList);
                        DisplaySub((int)currentSubMode);
                        PopUp.Singleton.CloseLoading();
                    }
                );
            }
            else
            {
                PopUp.Singleton.ShowError("您已經有追蹤過該用戶", false);
            }
        }
    }

    public void RemoveSubscribe(Subscribe subscribe)
    {
        int removeIndex = Utilities.INVALID;
        int i = 0;
        foreach (Subscribe s in mySubscribesList)
        {
            if (s.accountId == subscribe.accountId)
                removeIndex = i;
            i++;
        }

        PopUp.Singleton.ShowLoading();
        EESAPI.UnSubscribe(new SubscribeRequest(subscribe.accountId),
            (response) =>
            {
                mySubscribesList.Clear();
                foreach (EESUserData e in response.data)
                {
                    mySubscribesList.Add(new Subscribe(e.accountId, e.displayName));
                }
                FileManager.SaveFile(MySubscribeFileName, mySubscribesList);
                DisplaySub((int)currentSubMode);
                PopUp.Singleton.CloseLoading();
            },
            (error) =>
            {
                if (error == CommonError.Rejected)
                    PopUp.Singleton.ShowError("伺服器拒絕存取\r\n即將關閉程式", false, () => { Application.Quit(); });
                else if (error == CommonError.Timeout)
                {
                    PopUp.Singleton.ShowError("連線逾時", true, () => { AddSubscribe(subscribe); });
                }
                else if (error == CommonError.Unknown)
                {
                    PopUp.Singleton.ShowError("未知錯誤", false);
                }
                mySubscribesList.RemoveAt(removeIndex);
                FileManager.SaveFile(MySubscribeFileName, mySubscribesList);
                DisplaySub((int)currentSubMode);
                PopUp.Singleton.CloseLoading();
            }
        );
    }

    public void RefreshSubscribe()
    {
        Utilities.DeleteAllChildGameObject(MySubRoot);
        Utilities.DeleteAllChildGameObject(SubMeRoot);

        int i = 0;
        foreach (Subscribe s in mySubscribesList)
        {
            GameObject newGrid = Instantiate(SubPrefab);
            newGrid.GetComponent<SubscribeMono>().Initialize(i, s, true);
            Utilities.SetParentAndNormalize(newGrid.transform, MySubRoot);
            i++;
        }
        if (i == 0)
            NoMySub.SetActive(true);
        else
            NoMySub.SetActive(false);

        i = 0;
        foreach (Subscribe s in subscribeMeList)
        {
            GameObject newGrid = Instantiate(SubPrefab);
            bool find = false;
            foreach(Subscribe check in mySubscribesList)
            {
                if(check.accountId == s.accountId)
                {
                    find = true;
                }         
            }
            if(find)
                newGrid.GetComponent<SubscribeMono>().Initialize(i, s, true);
            else
                newGrid.GetComponent<SubscribeMono>().Initialize(i, s, false);
            Utilities.SetParentAndNormalize(newGrid.transform, SubMeRoot);
            i++;
        }
        if (i == 0)
            NoSubMe.SetActive(true);
        else
            NoSubMe.SetActive(false);
    }

    public void DisplaySub(int mode)
    {
        NoMySub.SetActive(false);
        NoSubMe.SetActive(false);
        SubMode setMode = (SubMode)mode;
        if (setMode != currentSubMode)
        {
            if (setMode == SubMode.My)
                TransitionHelper.TransitionIn(MovePanel);
            else
                TransitionHelper.TransitionOut(MovePanel);

            currentSubMode = (SubMode)mode;
        }

        if (currentSubMode == SubMode.My)
        {
            Caption.text = "我追蹤的用戶";
            MySubImage.color = SelectedColor;
            SubMeImage.color = Color.white;
            RefreshSubscribe();
        }
        else if (currentSubMode == SubMode.Me)
        {
            Caption.text = "追蹤我的用戶";
            MySubImage.color = Color.white;
            SubMeImage.color = SelectedColor;
            RefreshSubscribe();
        }
    }

    private void Start()
    {
        instance = this;
        currentSubMode = SubMode.My;
        mySubscribesList = FileManager.LoadFile<List<Subscribe>>(MySubscribeFileName);
        subscribeMeList = FileManager.LoadFile<List<Subscribe>>(SubscribeMeFileName);

        if (mySubscribesList == null)
        {
            mySubscribesList = new List<Subscribe>();
        }

        if (subscribeMeList == null)
        {
            subscribeMeList = new List<Subscribe>();
        }

        DisplaySub((int)SubMode.My);
        gameObject.SetActive(false);
    }
}
