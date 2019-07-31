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
    private float _cometSpawnTimer = 2f;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

    }
    private void Update()
    {
        if (!photonView.IsMine)
            return;
        if (planet == null)
        {
            planet = (PlanetManager)FindObjectOfType(typeof(PlanetManager));
        }
        if (gameActive)
        {
            _tick += Time.deltaTime;
            if(_tick >= _cometSpawnTimer)
            {
                _tick = 0;
                var randomPos = Random.onUnitSphere.normalized * planet.transform.localScale.x;

                ServerNetwork.instance.RequestInstantiateComet(randomPos);
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
