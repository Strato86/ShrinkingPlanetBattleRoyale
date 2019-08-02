using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviourPun , IPunObservable
{
    public bool isBGActive;
    public bool isCoundownActive;
    public string countdownValue;
    public string lastCarDestroyed;

    private int secondsToStart = 3;

    private void Start()
    {
        isBGActive = true;
        isCoundownActive = false;
        EventManager.AddEventListener(GameEvent.START_GAME, onStartGame);
        EventManager.AddEventListener(GameEvent.CAR_DESTROY, onCarDestroy);
        EventManager.AddEventListener(GameEvent.END_GAME, onEndGame);
    }

    private void onCarDestroy(object[] p)
    {
        lastCarDestroyed = (string)p[0];
    }

    private void onEndGame(object[] p)
    {
        EventManager.RemoveEventListener(GameEvent.END_GAME, onEndGame);
        isCoundownActive = true;
        countdownValue = (string)p[0] + " is the winner";
    }

    private void onStartGame(object[] p)
    {
        RemoveMenuBG();
        StartCountDown();
        EventManager.RemoveEventListener(GameEvent.START_GAME, onStartGame);
    }

    public void RemoveMenuBG()
    {
        isBGActive = false;
    }

    public void StartCountDown()
    {
        Debug.Log("Arranco a contar");
        StartCoroutine(StartGameCorroutine());
    }

    IEnumerator StartGameCorroutine()
    {
        Debug.Log("Arranco la corrutina para contar");
        isCoundownActive = true;

        for (int i = secondsToStart; i > 0; i--)
        {
            countdownValue = "Starting game in " + i;
            yield return new WaitForSeconds(1f);
        }

        isCoundownActive = false;
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(GameEvent.START_GAME, onStartGame);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isBGActive);
            stream.SendNext(isCoundownActive);
            stream.SendNext(countdownValue);
            stream.SendNext(lastCarDestroyed);
        }
        else
        {
            isBGActive = (bool)stream.ReceiveNext();
            isCoundownActive = (bool)stream.ReceiveNext();
            countdownValue = (string)stream.ReceiveNext();
            lastCarDestroyed = (string)stream.ReceiveNext();
        }
    }
}
