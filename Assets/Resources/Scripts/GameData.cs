using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData_SO", menuName = "ScriptableObjects/GameData Scriptable")]
public class GameData : ScriptableObject {

    public string dataName;
    public ScriptableObject player;
    public ScriptableObject goalObject;
    public ScriptableObject[] waypoints;
    public GameObject[] walls;
    public GameObject[] floorTiles;
    public int gridHeight, gridWidth;
    public static Color hintColour01, hintColour02;
    

    [Range(1,3)]
    public int difficulty;
    
    public static int count, northernEdge, easternEdge, southernEdge, westernEdge, horizDistance, vertDistance, xPosition;
    public static bool easyMode, goalFound, horizAligned, vertAligned, isForward, isRight, debugHoriz, debugVert, firstRow, firstColumn;
    public static bool showMenu = false;
    public static bool shortListed = false;
    public static bool firstRowFound = false;
    public static bool firstColFound = false;
    public const int NORTH = 0;
    public const int EAST = 1;
    public const int SOUTH = 2;
    public const int WEST = 3;
    public List<Vector3> path;

    private void Awake() {
        northernEdge = gridHeight / 2;
        easternEdge = gridWidth / 2;
        southernEdge = -northernEdge;
        westernEdge = -easternEdge;
    }

}
