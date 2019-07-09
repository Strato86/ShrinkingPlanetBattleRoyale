using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public PlanetManager planet;

    private float _horizontal;
    private float _vertical;

    private Rigidbody _rb;

    public float speed;
    public float rotationSpeed;


    void Start()
    {
        if(planet == null)
        {
            planet = GameMasterManager.instance.GetPlanet();
        }
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {

        //Movement

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

    }
}
