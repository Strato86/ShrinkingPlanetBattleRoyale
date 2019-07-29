using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterManager : MonoBehaviourPun
{
    public static GameMasterManager instance { get { return _instance; } }
    private static GameMasterManager _instance;

    public bool gameActive = false;
    public PlanetManager planet;

    private float _tick;
    private float _cometSpawnTimer = 10f;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        if(planet == null)
        {
            planet = (PlanetManager)FindObjectOfType(typeof(PlanetManager));
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;
        if (gameActive)
        {
            _tick += Time.deltaTime;
            if(_tick >= _cometSpawnTimer)
            {
                _tick = 0;
                var posX = Random.Range(-1, 1);
                var posY = Random.Range(-1, 1);
                var posZ = Random.Range(-1, 1);

                

                var pos = new Vector3(posX,posY,posZ) * Random.Range(50,55);
                ServerNetwork.instance.RequestInstantiateComet(pos);
            }
        }
    }

    public PlanetManager GetPlanet()
    {
        return planet;
    }

    public void StartGame()
    {
        gameActive = true;
    }

    public void EndGame()
    {
        gameActive = false;
    }
}
