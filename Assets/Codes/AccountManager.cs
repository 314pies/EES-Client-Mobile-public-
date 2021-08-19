using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EES.Utilities;
using EES.ClientAPIs;
using EES.ClientAPIs.ClientModules;
using FlipWebApps.BeautifulTransitions.Scripts.Transitions.Components.GameObject;

public class AccountManager : MonoBehaviour {

    //access account by AccountManager.Singleton.account
    private const string AccountFileName = "Account";

    private static AccountManager instance;
    public static AccountManager Singleton
    {
        get
        {
            return instance;
        }
    }

    private static Account account;
    public static  Account GetAccount
    {
        get
        {
            return account;
        }
    }
    public AccountType currentAccountStatus = AccountType.Guest;
    public AccountType CurrentAccountStatus { get { return currentAccountStatus; } }

    public GameObject LogoPanel;
    public GameObject LoginPanel;
    public GameObject LoadingInTheLogin;
    public GameObject SignUpPanel;
    public InputField EmailInputField;
    public InputField PasswordInputField;

    void Start()
    {
        instance = this;
        gameObject.SetActive(true);
        account = FileManager.LoadFile<Account>(AccountFileName);
        currentAccountStatus = AccountType.Guest;
        if (account == null)
        {
            //No file in the user's disk
            StartCoroutine(DisplayLogin());
        }
        else
        {
            if(account.AccountId == Utilities.INVALID)
            {
                StartCoroutine(DisplayLogin());
            }
            else
            {
                //Auto Login
                StartCoroutine(AutoLogin());
            }
        }
    }

    IEnumerator AutoLogin()
    {
        yield return new WaitForSeconds(2f);
        EmailInputField.text = account.Email;
        PasswordInputField.text = account.Password;
        Login(true);
    }

    IEnumerator DisplayLogin()
    {
        yield return new WaitForSeconds(2f);
        LoginPanel.SetActive(true);
        LogoPanel.GetComponent<TransitionMove>().TransitionIn();
        EmailInputField.text = "";
        PasswordInputField.text = "";
    }

    public void RejectToLogin()
    {
        gameObject.SetActive(true);
        Utilities.TransitionFade(LoginPanel);
        LogoPanel.GetComponent<TransitionMove>().TransitionIn();
        EmailInputField.text = "";
        PasswordInputField.text = "";
    }

    public void CreateAccount()
    {
        LogoPanel.GetComponent<TransitionFade>().TransitionOut();
        Utilities.TransitionOut(LoginPanel);
        Utilities.Transition(SignUpPanel);
    }

    public void Login(bool isAutoLogin)
    {
        string email = EmailInputField.text;
        string password = PasswordInputField.text;
        if (!isAutoLogin)
            PopUp.Singleton.ShowLoading();
        if (email.Length > 0 && password.Length > 0)
        {
            //invoke server login
            EESAPI.Login(new LoginRequest(email, password),
                (userData) =>
                {
                    account = new Account(userData.accountId, email, password, userData.displayName, userData.birthday, userData.isMale);
                    setAdmin();
                    FileManager.SaveFile(AccountFileName, account);
                    PopUp.Singleton.CloseLoading();
                },
                (error) =>
                {    
                    string errMsg = "";
                    if (error == LoginError.EmailNotFound || error == LoginError.WrongPassword)
                    {
                        errMsg = "請確認帳號密碼";
                        PopUp.Singleton.ShowError(errMsg, false);
                    }    
                    else if (error == LoginError.Timeout)
                    {
                        errMsg = "連線逾時";
                        PopUp.Singleton.ShowError(errMsg, true, () => { Login(false); });
                    } 
                    else if (error == LoginError.Unknown)
                    {
                        errMsg = "未知錯誤";
                        PopUp.Singleton.ShowError(errMsg, false);
                    }
                    PopUp.Singleton.CloseLoading();
                });
        }
        else
        {
            PopUp.Singleton.CloseLoading();
            PopUp.Singleton.ShowError("請確認欄位是否輸入正確", false);
        }
    }

    public void GuestLogin()
    {
        currentAccountStatus = AccountType.Guest;
        PopUp.Singleton.ShowAsking("遊客可能會無法使用部分功能\r\n要繼續嗎？", 
            null, 
            () => { MoveLogo(); } );
        account = new Account();
        UIManager.Singleton.SetAccountColors();
    }

    public void SigninLogin(int accountId, string email, string password, string displayName,
        DateTime birthday, bool isMale)
    {
        account = new Account(accountId, email, password, displayName, birthday, isMale);
        FileManager.SaveFile(AccountFileName, account);
        PopUp.Singleton.CloseLoading();
        currentAccountStatus = AccountType.Member;
        LogoPanel.GetComponent<TransitionFade>().TransitionIn();
        MoveLogo();
    }

    public void Logout()
    {
        LogoPanel.GetComponent<TransitionMove>().OutConfig.EndPosition = new Vector3(0, 240, 0);
        account = null;
    }

    public void BackToLogin()
    {
        Utilities.TransitionFade(LogoPanel);
        Utilities.TransitionFade(LoginPanel);
    }

    public bool setPassword(string password, string repeatedPassword)
    {
        return account.setPassword(password, repeatedPassword);
    }

    public bool setDisplayName(string displayName)
    {
        return account.setDisplayName(displayName);
    }

    public bool setBirthday(DateTime birthday)
    {
        return account.setBirthday(birthday);
    }

    public void setGender(bool isMale)
    {
        account.setGender(isMale);
    }

    private void setAdmin()
    {
        //invoke server to check admin of the exhibition or booth
        //server return exhibition list
        //ArrayList<Integer> exhibitionList = new ArrayList<Integer>();
        //for(int exhibitionId : exhibitionList) {
        //	account.setExhibitionHolder(exhibitionId);
        //}

        //server return booth list
        //ArrayList<Integer> boothList = new ArrayList<Integer>();
        //for(int boothId: boothList) {
        //account.setBoothHolder(boothId);
        EmailInputField.text = "";
        PasswordInputField.text = "";
        currentAccountStatus = AccountType.Member;
        UIManager.Singleton.SetAccountColors();
        MoveLogo();
    }

    public void MoveLogo()
    {
        Utilities.TransitionOut(LoginPanel);
        Utilities.TransitionOut(LoadingInTheLogin);
        TransitionMove LogoMove = LogoPanel.GetComponent<TransitionMove>();
        LogoMove.OutConfig.EndPosition = new Vector3(0, -420, 0);
        LogoMove.TransitionOut();
        StartCoroutine(DelayCloseLogin());
    }

    IEnumerator DelayCloseLogin()
    {
        yield return new WaitForSeconds(1f);
        ExhibitionManager.Singleton.Initialize();
        SubscribeManager.Singleton.GetSubscribe();
        gameObject.SetActive(false);
    }

    public enum AccountType
    {
        Guest,
        Member
    }
}

