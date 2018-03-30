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
    public Text Headline;
    public Text Text;

    public void Show(string headline, string text, Action yesAction=null, Action noAction=null)
    {
        EventSystem.current.SetSelectedGameObject(DialogPanel);
        DialogPanel.SetActive(true);
        Headline.text = headline;
        Text.text = text;
        YesButton.onClick.AddListener(() => CloseDialog(yesAction));
        NoButton.onClick.AddListener(() => CloseDialog(noAction));
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
