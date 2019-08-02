using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEditor;

public class CarController : MonoBehaviourPun, IPunObservable
{
    private const int CAMERA_DISTANCE = 10;

    public PlanetManager planet;
    public float speed;
    public float rotationSpeed;
    public GameObject myCamera;
    public bool isTaken;
    public GameObject GFX;
    public Animator _anim;
    public string playerID;
    private float _horizontal;
    private float _vertical;

    private Rigidbody _rb;
    private bool _isAlreadyMoving;

    private ServerNetwork _server;


    void Start()
    {
        if(planet == null)
        {
            planet = ServerNetwork.instance.planet;
        }
        _rb = GetComponent<Rigidbody>();
        StartCoroutine(AsignCameraDelay());
        _server = ServerNetwork.instance;
    }

    //Uso una corrutina cada 0.5 segundos para limitar el numero de llamados al servidor
    IEnumerator AsignCameraDelay()
    {
        while (!isTaken)
        {
            yield return new WaitForSeconds(0.5f);
            EventManager.DispatchEvent(GameEvent.CAR_SPAWN, photonView);
            Debug.Log("Asigno camara");
        }
    }

    /*private void Update()
    {

        _horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        _vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        transform.Translate(0, 0, _vertical);

        //LocalRotation
        transform.Rotate(0, rotationSpeed * Time.deltaTime * _horizontal, 0);

        //Ground Control
        var normal = (transform.position - planet.transform.position).normalized;
        transform.position = normal * planet.transform.localScale.x/2 ;

        //Stick to planet
        var toRotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
        transform.rotation = toRotation;

    }*/

    /*TODO: - Animator Move
    */

    public void Move(Vector3 axis)
    {
        if(_server != null)
        {
            if (GFX.activeSelf)
            {
                if (!planet)
                {
                    planet = _server.planet;
                }
                else if (!_isAlreadyMoving)
                {
                    //Movement
                    _isAlreadyMoving = true;
                    _horizontal = axis.x;
                    _vertical = axis.y;

                    transform.Translate(0, 0, _vertical);

                    //LocalRotation
                    transform.Rotate(0, rotationSpeed * Time.deltaTime * _horizontal, 0);

                    //Ground Control
                    var normal = (transform.position - planet.transform.position).normalized;
                    transform.position = normal * planet.transform.localScale.x / 2;

                    //Stick to planet
                    var toRotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
                    transform.rotation = toRotation;

                    //Start Coroutine to wait for next frame
                    StartCoroutine(WaitToMoveAgain());
                }
            }
        }
    }

    //Seteo la animacion
    public void SetAnimation(float horizontal)
    {
        if(_server != null)
        {
            if (GFX.activeSelf)
            {
                if (_anim)
                    _anim.SetFloat("curve", horizontal);
            }
        }
    }

    //Coroutine to move once per frame
    IEnumerator WaitToMoveAgain()
    {
        yield return new WaitForEndOfFrame(); 
        _isAlreadyMoving = false; //Can move again
    }

    public void RemoveCamera()
    {
        Destroy(myCamera);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_server)
        {
            Debug.Log("No server Bro!");
            return;
        }
        if (!photonView.IsMine)
        {
            Debug.Log("Cant controll this Bro!");
            return;
        }

        if(other.gameObject.layer == 10)
        {
            Debug.Log("Destroying this bitch!!");
            RequestActivateObject(false);
        }
    }

    //IPunObservable implementation
    //To sync isTaken variable
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isTaken);
            stream.SendNext(playerID);
        }
        else
        {
            isTaken = (bool)stream.ReceiveNext();
            playerID = (string)stream.ReceiveNext();
        }
    }

    public void RequestActivateObject(bool active)
    {
        gameObject.SetActive(active);
        StopAllCoroutines();
        _server.SetLoser(this);
        _server.ParticleRequestInstantiate("CarExplotion", transform);
        var text = FindObjectOfType<UiControllerObserver>().countDownText;
        text.text = "You Lost!";
        text.enabled = true;
        photonView.RPC("DeactivateGFXObject", RpcTarget.OthersBuffered, active);
        EventManager.DispatchEvent(GameEvent.CAR_DESTROY, playerID);
    }

    [PunRPC]
    void DeactivateGFXObject(bool active)
    {
        GFX.SetActive(active);
        enabled = false;
    }
}
