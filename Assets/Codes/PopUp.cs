using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EES.Utilities;

public class PopUp : MonoBehaviour
{

    private static PopUp instance;
    public static PopUp Singleton
    {
        get
        {
            return instance;
        }
    }

    public GameObject LoadingPanel;
    public GameObject MessagePanel;
    public Text MessageText;
    public GameObject ErrorPanel;
    public Text ErrorText;
    public GameObject RetryPanel;
    public Text RetryText;
    public GameObject AskingPanel;
    public Text AskingText;

    private Action confirmAction;
    private Action cancelAction;
    private Action errorAction;
    private Action retryAction;

    // Use this for initialization
    void Awake()
    {
        instance = this;
    }

    public void ShowLoading()
    {
        Utilities.Transition(LoadingPanel);
    }

    public void CloseLoading()
    {
        Utilities.TransitionOut(LoadingPanel);
    }

    public void ShowMessage(string message, Action onClicked = null)
    {
        MessageText.text = message;
        confirmAction = onClicked;
        Utilities.Transition(MessagePanel);
    }

    public void ShowError(string errorMessage, bool retryEnable, Action onClicked = null)
    {
        if (retryEnable)
        {
            RetryText.text = errorMessage;
            retryAction = onClicked;
            Utilities.Transition(RetryPanel);
        }
        else
        {
            ErrorText.text = errorMessage;
            errorAction = onClicked;
            Utilities.Transition(ErrorPanel);
        }
    }

    public void ShowAsking(string message, Action onCancelClicked = null, Action onConfirmClicked = null)
    {
        AskingText.text = message;
        cancelAction = onCancelClicked;
        confirmAction = onConfirmClicked;
        Utilities.Transition(AskingPanel);
    }

    #region For Inspecter
    public void OnConfirmClicked()
    {
        if (confirmAction != null)
            confirmAction();
    }

    public void OnCancelClicked()
    {
        if (cancelAction != null)
            cancelAction();
    }

    public void OnErrorClicked()
    {
        if (errorAction != null)
            errorAction();
    }

    public void OnRetryClicked()
    {
        if (retryAction != null)
            retryAction();
    }

    public void Close(GameObject gameObject)
    {
        Utilities.TransitionOut(gameObject);
    }
    #endregion
}
