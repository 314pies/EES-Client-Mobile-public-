using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EES.ClientAPIs;
using EES.ClientAPIs.ClientModules;
using Newtonsoft.Json;
using System;

public class ConntectTest : MonoBehaviour {

    public Button LoginButton;
    public Button SignUpButton;
    public Button ClearButton;
    public InputField Email;
    public InputField Password;
    public InputField Name;
    public Toggle IsMale;
    public Text Console;

    public void Login()
    {
        LoginRequest loginRequest = new LoginRequest(Email.text, Password.text);
        Console.text = JsonConvert.SerializeObject(loginRequest);
        if (Email.text != "" && Password.text != "")
            EESAPI.Login(loginRequest,
                (userData) =>
                {
                    Console.text += "\r\n";
                    Console.text += JsonConvert.SerializeObject(userData);
                },
                (error) =>
                {
                    Console.text += "\r\n";
                    Console.text += JsonConvert.SerializeObject(error);
                });
        else
            Console.text = "Field cannot be null.";
    }

    public void SignUp()
    {
        SignRequest userData = new SignRequest(Email.text, Password.text, Name.text, DateTime.Now, IsMale.isOn);
        Console.text = JsonConvert.SerializeObject(userData);
        if (Email.text != "" && Password.text != "" && Name.text != "")
            EESAPI.SignUp(new SignRequest(Email.text, Password.text, Name.text, DateTime.Now, IsMale.isOn),
                (accountId) =>
                {
                    Console.text += "\r\nAccountId:" + accountId + "SignSuccess";
                    Console.text += "\r\nSignSuccess";
                },
                (error) =>
                {
                    Console.text += "\r\n";
                    Console.text += JsonConvert.SerializeObject(error);
                });
        else
            Console.text = "Field cannot be null.";
    }

    public void Clear()
    {
        Email.text = "";
        Password.text = "";
        Name.text = "";
        IsMale.isOn = false;
        Console.text = "";
    }
}
