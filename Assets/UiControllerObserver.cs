using Photon.Pun;
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
    private string _lastCarDestroyed = "";

    private bool lost;

    public Image menuBG;
    public Text countDownText;

    public Text youLose;


    private void Start()
    {
        youLose.enabled = false;
        EventManager.AddEventListener(GameEvent.CAR_DESTROY, onCarDestroy);
    }

    private void onCarDestroy(object[] p)
    {
        var destroyedID = (string)p[0];
        if(destroyedID == PhotonNetwork.LocalPlayer.UserId)
        {
            EventManager.RemoveEventListener(GameEvent.CAR_DESTROY, onCarDestroy);
            SetLoseText();
        }
    }

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
            _lastCarDestroyed = _ui.lastCarDestroyed;
        }

        SetUI();
    }

    private void SetUI()
    {
        menuBG.gameObject.SetActive(_isMenuBGActive);
        countDownText.enabled = _isCountdownActive;
        countDownText.text = _countdown;
        Debug.Log(_lastCarDestroyed + " " + PhotonNetwork.LocalPlayer.UserId + " " + _lastCarDestroyed == PhotonNetwork.LocalPlayer.UserId);
        if(_lastCarDestroyed == PhotonNetwork.LocalPlayer.UserId)
        {
            SetLoseText();
        }
    }

    public void SetLoseText()
    {
        youLose.enabled = true;
    }
}
