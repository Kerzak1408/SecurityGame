using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserDialog : MonoBehaviour
{
    public  static UserDialog Instance { get; private set; }

    public GameObject DialogPanel;
    public Button YesButton;
    public Button NoButton;
    public Button OkButton;
    public Text Headline;
    public Text Text;

    public void ShowYesNo(string headline, string text, Action yesAction=null, Action noAction=null)
    {
        ShowCommon(headline, text, false);
        YesButton.onClick.AddListener(() => CloseDialog(yesAction));
        NoButton.onClick.AddListener(() => CloseDialog(noAction));
    }

    public void ShowOk(string headline, string text, Action okAction=null)
    {
        ShowCommon(headline, text, true);
        OkButton.onClick.AddListener(() => CloseDialog(okAction));
    }

    private void ShowCommon(string headline, string text, bool okActive)
    {
        EventSystem.current.SetSelectedGameObject(DialogPanel);
        DialogPanel.SetActive(true);
        Headline.text = headline;
        Text.text = text;
        ShowButtons(okActive);
    }

    private void ShowButtons(bool okActive)
    {
        YesButton.gameObject.SetActive(!okActive);
        NoButton.gameObject.SetActive(!okActive);
        OkButton.gameObject.SetActive(okActive);
    }

    private void CloseDialog(Action action)
    {
        DialogPanel.SetActive(false);
        if (action != null)
        {
            action();
        }
        YesButton.onClick.RemoveAllListeners();
        NoButton.onClick.RemoveAllListeners();
        OkButton.onClick.RemoveAllListeners();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
