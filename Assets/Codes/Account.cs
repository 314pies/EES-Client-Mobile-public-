using System;
using System.Collections;
using System.Collections.Generic;
using EES.ClientAPIs.ClientModules;
using EES.Utilities;

public class Account
{
    private int accountId = Utilities.INVALID;
    private string email;
    private string password;
    private string displayName;
    private DateTime birthday;
    private bool isMale;

    #region Property
    public int AccountId { get { return accountId;} set { setAccountId(value); } }
    public string Email { get { return email; } set { if (value != null && value.Length > 0) email = value; } }
    public string Password { get { return password; } set { if (value != null && value.Length > 0) password = value; } }
    public string DisplayName { get { return displayName; } set { if (value != null && value.Length > 0) displayName = value; } }
    public DateTime Birthday { get { return birthday; } set { if (value <= DateTime.Now) birthday = value; } }
    public bool IsMale { get { return isMale; } set { isMale = value; } }
    #endregion

    private bool initialized = false;
    private List<int> exhibitionHolder;
    private List<int> boothHolder;

    public Account(int accountId, String email, String password, String displayName, DateTime birthday, bool isMale)
    {
        if (email != null && email != "" && password != null && password != "")
        {
            this.accountId = accountId;
            this.email = email;
            this.password = password;
            this.displayName = displayName;
            this.birthday = birthday;
            this.isMale = isMale;
        }
    }

    public Account()
    {
        accountId = Utilities.INVALID;
    }

    public void setAccountId(int accountId)
    {
        if (!initialized && accountId != Utilities.INVALID)
        {
            this.accountId = accountId;
            initialized = true;
        }
    }

    public bool setPassword(string password, string repeatedPassword)
    {
        if (password.Equals(repeatedPassword))
        {
            this.password = password;
            return true;
        }
        return false;
    }

    public bool setDisplayName(String displayName)
    {
        if (displayName.Length > 0)
        {
            this.displayName = displayName;
            return true;
        }
        return false;
    }

    public bool setBirthday(DateTime birthday)
    {
        if (birthday < DateTime.Now)
        {
            this.birthday = birthday;
            return true;
        }
        return false;
    }

    public void setGender(bool isMale)
    {
        this.isMale = isMale;
    }

    public void addExhibitionHolder(int exhibitionId)
    {
        exhibitionHolder.Add(exhibitionId);
    }

    public void addBoothHolder(int boothId)
    {
        boothHolder.Add(boothId);
    }
}