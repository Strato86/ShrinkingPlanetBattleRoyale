using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometBehaviour : MonoBehaviourPun
{
    private float _speed;
    public float gravityConstant;

    void Update()
    {
        var distance = Vector3.Distance(transform.position, Vector3.zero);
        _speed = gravityConstant / (distance * distance);
        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        RequestDestroyComet(other);
    }

    public void RequestDestroyComet(Collider other)
    {
        if (!photonView.IsMine)
            return;
        gameObject.SetActive(false);
        if(other.gameObject.layer != 10)
        {
            var position = transform.position.normalized * ServerNetwork.instance.planet.transform.localScale.x/2;
            ServerNetwork.instance.RequestDestroyComet(gameObject);
            ServerNetwork.instance.CraterRequestInstantiate(position);
        }
    }


}
