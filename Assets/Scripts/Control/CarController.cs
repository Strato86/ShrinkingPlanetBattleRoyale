using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CarController : MonoBehaviourPun
{
    public PlanetManager planet;
    public float speed;
    public float rotationSpeed;
    public GameObject myCamera;

    private float _horizontal;
    private float _vertical;

    private Rigidbody _rb;
    private bool _isAlreadyMoving;

    

    void Start()
    {
        if(planet == null)
        {
            //planet = FindObjectOfType<PlanetManager>();
            planet = ServerNetwork.instance.planet;
        }
        _rb = GetComponent<Rigidbody>();
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
        if (!planet)
        {
            planet = ServerNetwork.instance.planet;
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
}
