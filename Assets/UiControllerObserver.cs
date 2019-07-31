using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiControllerObserver : MonoBehaviour
{
    private UIController _ui;
    private bool _isMenuBGActive = true;
    private bool _isCountdownActive = false;
    private string _countdown = "";

    public Image menuBG;
    public Text countDownText;

    void Update()
    {
        if (!_ui)
        {
            _ui = FindObjectOfType<UIController>();
        }
        else
        {
            _isMenuBGActive = _ui.isBGActive;
            _isCountdownActive = _ui.isCoundownActive;
            _countdown = _ui.countdownValue;
        }

        SetUI();
    }

    private void SetUI()
    {
        menuBG.gameObject.SetActive(_isMenuBGActive);
        countDownText.enabled = _isCountdownActive;
        countDownText.text = _countdown;
    }
}
