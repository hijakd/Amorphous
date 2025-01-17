using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameDataScriptable")]
public class GameData : ScriptableObject {

    public string name;
    public GameObject goalObject;
    public GameObject player;
    public int gridHeight, gridWidth;

    [Range(1,3)]
    public int difficulty;

}
