using System;
using UnityEngine;

public class GameDataScript : MonoBehaviour {

    [SerializeField] private GameData gameData;
    
    void Start()
    {
        Debug.Log("GameData loaded");    
    }


}
