using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    [Range(0.9990f,1f)]
    public float shrinkSpeed;

    public float minSize;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (transform.localScale.x > minSize)
        {
            transform.localScale *= shrinkSpeed;
        }
    }
}
