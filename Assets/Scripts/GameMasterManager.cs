using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMasterManager : MonoBehaviour
{
    public static GameMasterManager instance { get { return _instance; } }
    private static GameMasterManager _instance;

    public bool gameActive = false;
    public PlanetManager planet;

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

    public PlanetManager GetPlanet()
    {
        return planet;
    }

    private void StartGame()
    {
        gameActive = true;
    }

    private void EndGame()
    {
        gameActive = false;
    }
}
