using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PlayerCamera : MonoBehaviour
{
    private const int CAMERA_DISTANCE = 10;
    private bool _isAsigned;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.enabled = false;
        EventManager.AddEventListener(GameEvent.CAR_SPAWN, onCarSpawn);
    }

    

    private void onCarSpawn(object[] p)
    {
        if (!_isAsigned)
        {
            var carControllers = FindObjectsOfType<CarController>();
            foreach (var car in carControllers)
            {
                if (!car.isTaken)
                {
                    ServerNetwork.instance.PlayerAsignCamera(PhotonNetwork.LocalPlayer);
                    cam.enabled = true;
                    _isAsigned = true;
                    transform.position = car.transform.position + car.transform.up * CAMERA_DISTANCE;
                    transform.rotation = Quaternion.identity;
                    transform.SetParent(car.transform);
                    transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                }
            }
        }
        EventManager.RemoveEventListener(GameEvent.CAR_SPAWN, onCarSpawn);
    }

    private void OnDestroy()
    {
        EventManager.RemoveEventListener(GameEvent.CAR_SPAWN, onCarSpawn);
    }
}
