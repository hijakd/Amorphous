using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData_scb", menuName = "Scriptable Objects/GameData Scriptable")]
public class GameData : ScriptableObject {

    public string name;
    public GameObject goalObject;
    public GameObject player;
    public int gridHeight, gridWidth;

    [Range(1,3)]
    public int difficulty;

}
