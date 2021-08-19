using EES.ClientAPIs.ClientModules;
using EES.ClientAPIs;
using EES.Native;
using EES.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignUpManager : MonoBehaviour
{

    private static SignUpManager instance;
    public static SignUpManager Singleton
    {
        get
        {
            return instance;
        }
    }

    public InputField Email;
    public InputField Password;
    public InputField RepeatedPassword;
    public InputField DisplayName;
    public Text Gender;
    public Text Birthday;


    public Image[] Icons;
    public Text[] Captions;
    public Text[] Contents;

    public Color NormalIcon;
    public Color NormalCaption;
    public Color NormalContent;
    public Color Wrong;

    private bool isMale = true;
    private DateTime birthday;

    private const int EmailField = 0;
    private const int PasswordField = 1;
    private const int RepeatedPasswordField = 2;
    private const int DisplayNameField = 3;
    private const int BirthdayField = 4;
    private const int TotalFieldCount = 5;

    private void Start()
    {
        instance = this;
        Initialize();
    }

    public void Initialize()
    {
        Email.text = "";
        Password.text = "";
        RepeatedPassword.text = "";
        DisplayName.text = "";
        Gender.text = "先生";
        isMale = true;
        birthday = new DateTime(DateTime.Now.Year - 20, DateTime.Now.Month, DateTime.Now.Day);
        Birthday.text = DateTime.Now.Year - 20 + " 年 " + DateTime.Now.Month + " 月 " + DateTime.Now.Day + " 日";

        foreach (Image image in Icons)
        {
            image.color = NormalIcon;
        }
        foreach (Text text in Captions)
        {
            text.color = NormalCaption;
        }
        foreach (Text text in Contents)
        {
            text.color = NormalContent;
        }
    }

    public void SignUp()
    {
        string email = Email.text;
        string password = Password.text;
        string repeated = RepeatedPassword.text;
        string name = DisplayName.text;

        if (!email.Contains("@") || !email.Contains("."))
        {
            PopUp.Singleton.ShowError("請輸入正確的 E-mail 欄位", false, () => { SetWrongColor(EmailField); });
            return;
        }

        if (password.Length < 6 || password.Length > 20)
        {
            PopUp.Singleton.ShowError("密碼須為 6 ~ 20 個英數字", false, () => { SetWrongColor(PasswordField); });
            return;
        }

        if (!password.Equals(repeated))
        {
            PopUp.Singleton.ShowError("密碼與確認密碼不一致", false, () =>
            {
                SetWrongColor(PasswordField);
                SetWrongColor(RepeatedPasswordField);
            });
            return;
        }

        if (name.Length == 0 || name == "")
        {
            PopUp.Singleton.ShowError("暱稱不可為空白", false, () => { SetWrongColor(DisplayNameField); });
            return;
        }

        if (birthday > DateTime.Now)
        {
            PopUp.Singleton.ShowError("生日輸入錯誤", false, () => { SetWrongColor(BirthdayField); });
            return;
        }

        PopUp.Singleton.ShowLoading();
        EESAPI.SignUp(new SignRequest(email, password, name, birthday, isMale),
            (accointId) =>
            {
                PopUp.Singleton.CloseLoading();
                AccountManager.Singleton.SigninLogin(accointId, email, password, name, birthday, isMale);
                Initialize();
                Utilities.TransitionOut(gameObject);
            },
            (error) =>
            {
                PopUp.Singleton.CloseLoading();

                string errMsg;
                if (error == RegisterError.EmailIsExist)
                {
                    errMsg = "此E-mail已經存在";
                    PopUp.Singleton.ShowError(errMsg, false);
                }
                else if (error == RegisterError.InvalidInput)
                {
                    errMsg = "輸入資料格式有誤";
                    PopUp.Singleton.ShowError(errMsg, false);
                }
                else if (error == RegisterError.DataInsertFailed)
                {
                    errMsg = "存入資料庫失敗";
                    PopUp.Singleton.ShowError(errMsg, false);
                }
                else if (error == RegisterError.Timeout)
                {
                    errMsg = "連線逾時";
                    PopUp.Singleton.ShowError(errMsg, true, () => { SignUp(); });
                }
                else if (error == RegisterError.Unknown)
                {
                    errMsg = "未知錯誤";
                    PopUp.Singleton.ShowError(errMsg, false);
                }
            });
    }

    public void CloseSignUpPanel()
    {
        Initialize();
        Utilities.TransitionOut(gameObject);
        AccountManager.Singleton.BackToLogin();
    }

    public void SetNormalColor(int target)
    {
        if (target >= 0 && target < TotalFieldCount)
        {
            Icons[target].color = NormalIcon;
            Captions[target].color = NormalCaption;
            Contents[target].color = NormalContent;
        }
    }

    public void SetWrongColor(int target)
    {
        if (target >= 0 && target < TotalFieldCount)
        {
            Icons[target].color = Wrong;
            Captions[target].color = Wrong;
            Contents[target].color = Wrong;
        }
    }

    public void OnBirthdayClicked()
    {
        SetNormalColor(BirthdayField);
        MobileNative.OnPickDateClick(
            birthday,
            (year, month, day) =>
            {
                Birthday.text = year + " 年 " + month + " 月 " + day + " 日";
                birthday = new DateTime(year, month, day);
            },
            () =>
            {
                birthday = new DateTime(DateTime.Now.Year - 20, DateTime.Now.Month, DateTime.Now.Day);
                Birthday.text = birthday.Year + " 年 " + birthday.Month + " 月 " + birthday.Day + " 日";
            }
        );
    }

    public void OnGenderClicked()
    {
        string[] items = { "先生", "小姐" };
        int defaultIndex;
        if (isMale)
            defaultIndex = 0;
        else
            defaultIndex = 1;

        MobileNative.OnRadioClick(defaultIndex, items,
            (selectedIndex) =>
            {
                if (selectedIndex == 0)
                {
                    isMale = true;
                    Gender.text = "先生";
                }
                else
                {
                    isMale = false;
                    Gender.text = "小姐";
                }
            }
        );
    }
}
