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
        RequestDestroyComet();
    }

    public void RequestDestroyComet()
    {
        gameObject.SetActive(false);
        photonView.RPC("DestroyComet", RpcTarget.OthersBuffered);
        ServerNetwork.instance.CraterRequestInstantiate(transform.position);
    }

    [PunRPC]
    void DestroyComet()
    {
        gameObject.SetActive(false);
    }
}
